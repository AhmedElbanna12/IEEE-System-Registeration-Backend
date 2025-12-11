using QRCoder;

namespace IEEE_RegSys.Helpers
{
    public class QRHelper
    {
        public byte[] GenerateQrBytes(string payload)
        {
            using var qrGen = new QRCodeGenerator();
            using var data = qrGen.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            using var qr = new PngByteQRCode(data);
            return qr.GetGraphic(20);
        }
    }
}
