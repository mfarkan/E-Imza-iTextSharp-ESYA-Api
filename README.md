#  E-Imza-iTextSharp-KAMUSM-Api

ITextSharp kütüphanesi ile TÜBİTAK KamuSM ESYA API yardımıyla E-imza bilgileri alınarak PDF bilgilerinin imzalandığı Windows Forms uygulaması.


## İmzalanmış Pdf Çıktısı
![2 imzalı bir pdf çıktısı.](https://image.ibb.co/eBbHGo/image.png)

## E-Imza Bilgisinin alınması.

```c#
SmartCardManager smartCardManager = SmartCardManager.getInstance();
var smartCardCertificate = smartCardManager.getSignatureCertificate(false,false);
var signer = smartCardManager.getSigner(request.DonglePassword, smartCardCertificate);
CERTIFICATE = smartCardCertificate.asX509Certificate2();
externalSignature = new SmartCardSignature(signer, CERTIFICATE, "SHA-256");
```
> **Not:** Burada kullanılan kod parçacığı **KamuSM'in SmardCardManager.cs** isimli sınıfından alınmıştır.

## Imzalanmış PDF'in Imza bilgisinin Kontrolü

![Imzalı bir PDF'in bilgisi](https://image.ibb.co/jhG9Kp/signature.png)

## Uygulama Uyarı Mesajı

![Uyarı Mesajı](https://image.ibb.co/e1PJC9/valid_imza.jpg)

## Imza Bilgisinin Kontrolü 

```c#
PdfReader reader = new PdfReader(pdfContent);
AcroFields fields = reader.AcroFields;
List<String> names = fields.GetSignatureNames();

for (int i = 1; i < names.Count + 1; i++)
 {//Birden fazla imza olabildiği için döngüyle her imza kontrol edildi.
  string temp = string.Empty;
  PdfPKCS7 pkcs7 = fields.VerifySignature(names[i - 1]);
  var result = pkcs7.Verify();
  if (result)
  {
      temp = string.Format("{0}.imza geçerli.", i);
  }
  else
  {
      temp = string.Format("{0}.imza geçersiz.", i);
  }
  message += temp;
 }
            
```


### Kaynaklar ;

[KamuSM ESYA API](https://yazilim.kamusm.gov.tr/esya-api/doku.php)

[iTextSharp API](https://developers.itextpdf.com/itext-5-examples)
