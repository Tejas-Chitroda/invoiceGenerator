using Invoice_Generator.DTOs;
using Invoice_Generator.Models;
using Invoice_Generator.Services.Interfaces;
using Invoice_Generator.UoW;

namespace Invoice_Generator.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {

        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddInvoiceAsync(InvoiceRequestDto invoice)
        {
            var invoiceModel = new Invoice
            {
                CustomerId = invoice.CustomerId,
                InvoiceDate = DateTime.UtcNow,
                InvoiceDetails = new List<InvoiceDetail>()
            };

            decimal grandTotal = 0;
            var today = DateTime.UtcNow.Date;
            decimal SubTotalForInvoice = 0;
            decimal TaxTotalForInvoice = 0;

            foreach (var item in invoice.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);

                if (product == null)
                    throw new ArgumentException($"Product with ID {item.ProductId} does not exist.");

                var productPrice = (await _unitOfWork.ProductPrices.FindAsync(p =>
                             p.ProductId == item.ProductId &&
                             !p.IsDefault &&
                             p.EffectiveFrom != null &&
                             p.EffectiveTo != null &&
                             today >= p.EffectiveFrom.Value.Date &&
                             today <= p.EffectiveTo.Value.Date))
                             .FirstOrDefault()?.Price;

                if (productPrice == null || productPrice == 0)
                {
                    productPrice = (await _unitOfWork.ProductPrices
                             .FindAsync(p => p.ProductId == item.ProductId && p.IsDefault))
                             .FirstOrDefault()?.Price ?? 0;
                }

                var qty = item.Quantity;
                var subTotal = productPrice * qty;
                var taxAmt = subTotal * (product.TaxPercentage) / 100;
                var totalAmt = subTotal + taxAmt;

                invoiceModel.InvoiceDetails.Add(new InvoiceDetail
                {
                    ProductId = item.ProductId,
                    Quantity = qty,
                    Rate = (decimal)productPrice,
                    SubTotal = (decimal)subTotal,
                    Tax = (decimal)taxAmt,
                    Total = (decimal)totalAmt,
                    GrandTotal = (decimal)totalAmt
                });

                invoiceModel.TaxTotal = product.TaxPercentage;
                grandTotal += (decimal)totalAmt;
                SubTotalForInvoice += (decimal)subTotal;
                TaxTotalForInvoice += (decimal)taxAmt;
            }

            invoiceModel.GrandTotal = grandTotal;
            invoiceModel.SubTotal = SubTotalForInvoice;
            invoiceModel.TaxTotal = TaxTotalForInvoice;

            await _unitOfWork.Invoices.AddAsync(invoiceModel);

            foreach (var detail in invoiceModel.InvoiceDetails)
            {
                await _unitOfWork.InvoiceDetails.AddAsync(detail);
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> DeleteInvoiceAsync(int id)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(id);
            if (invoice == null) return false;

            _unitOfWork.Invoices.Delete(invoice);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
        {
            return await _unitOfWork.Invoices.GetAllAsync();
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
        {
            return await _unitOfWork.Invoices.GetByIdAsync(id);
        }

        public async Task<IEnumerable<InvoiceDetail>> GetInvoiceDetailByInvoiceId(int invoiceId)
        {
            return await _unitOfWork.InvoiceDetails.FindAsync(i => i.InvoiceId == invoiceId);
        }
    }
}

