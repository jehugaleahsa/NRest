using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRest.MultiPart;

namespace NRest.Tests
{
    [TestClass]
    public class MultiPartParserTester
    {
        #region Constants

        private const string boundary = "--------------------------------abc123def456ghi789";
        private const string preamble = @"This is a preamble.
It can be arbitrarily long with random newlines throughout. I am going to
make this preamble exceptionally long so that it exceeds the buffer size. I only need to make sure
it translates to a byte array of over 4096 when converted using UTF8. Since I am using ASCII
characters to write it, I need to make this preamble at least 4096 characters long.
This is going to take a lot of writing because 4096 characters is really big. I could have gone in
and made the size of buffer variable, but I don't see any reason to make the buffer size configurable.
At some point, I am going to get tired of writing this ridiculously long preamble. It is scary to
think I am going to have to more or less write something this long to properly test all of the other
pieces of the parser. Ugh. Maybe I should just go online and find a book to copy or some HTML or
something.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
uffer size configurable.
At some point, I am going to get tired of writing this ridiculously long preamble. It is scary to
think I am going to have to more or less write something this long to properly test all of the other
pieces of the parser. Ugh. Maybe I should just go online and find a book to copy or some HTML or
something.43/FAaMBa8ndcHlGPv5j86vUOnsHvv0VTYqKJm+jDzpffNwXtsAObWZHsKwEeKQ8oERo4qftOCWCsLMYdkRZOWA+5
8EiSzzqxswUwA5QENXmToYqOy2TckS3lQYM9fch7XZLxZHjGlUIfRva2HcWMaDeMii+Lv4zq3R8a7Y23tDV8tKXBEpEAjQwIr4R
uJnMoGNlLsLPY793GBocZq6e0Z5LBeWkfi1VmS/zjQs/IvLbsMpT0LAOBo8TIOwvKJyFkVAfxrO0JyE1EJST29WzN/UoQbbjSwH
uPf9WkV5BJXGTbvu7IFw/JoipHcAbeiu5YOj3B+fmoUZPMYmzXZbC9Ooj/2za+BeY6BotOi1A08LTnTB2Sh6KrM21a+7Ti8jx5i
nW93u+3mjQXh/MuGYYDl7H38BmTKrMmVOfJYwpxFxRO1MEattGFb7Ipuv+vfNmnFyQDKsIi5p/CV4oHrnbp5nCe3wTE6SDgMoCna
a+zdxfOYgwmLixSbC4hWF8HvYE7f5SPEJ2Uoi4DIzhBe70wDyCyeUwqg7hETHMWo5CBmHT/bCnbJGZ6OcAd4wrz0PCw8Ul+lUR00
LwcCoQqvf6dQYqDQg9TVSq6OD+MDOCtUHuCpfJHQsMwSltt/tmiT3vBQKFAMI7hHZwehJ0E6MPbbvpIqBgMVf6Hkve0JZYQ16jDi
sw/LR/soYgWKP5l62GXklAfWedjtSqiuJ2XbPgGDez0QV43nEGam8ZP9i0AYedV3JNGMb3BX9WhnHSCFXSQEBrRcWoDn/YORP3g0
/89Ylb2AVBkv9wvMyFIVf/NGEaR/6YSLvB9Unydd+z/4EwWpHndzBQkK1A+on5I+wn2ASx3TtQY6zOmeO7Tx0gvb1KVMfGF70xyr
dRV00yx9FdypzaASFF4PbkGd1NlECPID3Lrs4kYMBHgBOE/wtenNIRXoSBRbybBXTR4f7g4zV2wTcQZhgCOGnbk4ANweDqhn0LOz
a8DtqAL6lZU3/CZ5zeDT7UTTW5DiUapbI7jkOU5luB1Kj+urJckDZvqeXqGSzkL7OiFThHUSwRoWgoRaFlxWwjQZL031OLeL3+Sm
U/L4yFV16Q8dAiOYN77RhJM0uVCeN5x9ea71BgF517SAfH0XHco+1D1dSbAAcOefE6QURyaUCCxmi062gNL9kzp/F7sxymbZBdUT
MO3sbQi3VeJtZZCN/GRQoIJ7eFNaX6aFgWk8bVk9OzFBcMc+ii5F/yrgkD2Z4RtW4RnkXZ2ByNFHVtufiviZICpfTIudIiZkNXko
ac9o66BN1XxPWO2jz7fl5yNCpnQyV2ATOqSfuc7jcBgUaV8R3ns7QaV8Wdc8u3zDtzsDEZ8E5hJfVaD7viu9W9jFn2ZY1Bi4F1TO
tg2Y1o83zAKT4G6JD3M8sojKT5N5zv3+dxwWetMiSvpKNiAUxn8hw/IpKWt6TI/jLCGP/dfBRPcPmm9jEVEvcZVkTtlDJ+wZmHuh
UKWChDqcM16xkl0ieOE5BSwfzqKQ+bfGFg2mn/Q1dlVcH/RRjRYkxu4Cfd9IMcX4GiKwCdnJAFbtvG485ZNajCwHxlj854gV6vfE
novYjdQ3voXBdXyLgx+dY8Cn9+FKp/GzQGts+zKqqh7iBHHbFT6J9ScNmQavfENJX3O3beMGefaaJU142rNTJstdmY6tsk6SLgK6
TQ7rA8QoE6u+40w724p7QO/hERpzunI7UeLEm501JU/gXDznryF5QMcXpDOPIc30InFuoQzoRoKcr6HhY3gOib4MS39dk5HOZ9BT
XAY+vDAUwHkrxoRC+BWqSw9/l+1PGSp1gtZOlo7vkDkslOp8XfNxzJMO4rM5uVNps1iCXt/jFELruSlZxjnqPDZLFmMrLe0L/ctK
MCZ3S8mC5OM2XTFpOYCnkKTaF/ZpFLZ87qLwToVrhlEEqNQcdH1v1uFGs9704dzSg51IMeXE4St/+m0WgpZdytbgVJC8mxwnakEV
EmDXd/zWHcr4I1R7T7eonHoQSURK6PkIFJ08JehxEMeUv97KqHJgQHTbZadyZRTpA/6SFyD8yiln32NfmffSdwKc1o/o7eQHr/jf
b4NBha+IUyHp3wmWp/WlUtkfALZ9CO6U5Y6FABYammcoHVqmFkmz10bV6JgI9j6aM5sa+J9CUmaUvJAQ80ML30SSboqa/zyPWzlv
pyBGm/9f+syK/6HX5br0EWm5ktPzyglTZoej/0Rum51l0qs2ThCgwPh2gO+iyojfpt3C1bhVa+0W8zVnvAuIHGQEYgMPvdNZCNHr
fTkpsKv+1Ehmm8XlEHbOL0rrOQT8/mSLnliFz/j6eczWee6j7TKJaOezEYCeL1GcQY6Ifd/c7o7UrhJDeRRfbCH5DpaIGjIIUcAt
Sdzj7X8G+X5qcgvtCYtv308gRf/TU7VupRC7BDdcgBwPr6Sh4MCTTI+V9PomC/UbGDry6BvQWeYndUhuS+VHDeHv3Vnmze6OjPlw
PV67SJkbr/v8aZgAP1W+enuINPCaTvcINi6rJFpMdFsSYByXfDyvQqvpWqRJ3630OENy9kgI1e5nowJqcIuhr+VM7d9JFw4+2PJU
sPRCvlaApuwGyX4DGZouL8k6RNIgR6q/DOHOuyFFRC1VpBT6JOSr2dmvmtZ3OdRFKuc1BJQgNGlZg6F0wevKwYVArWV1NArUjmr4
cKzUR9MdiZA6AE+ZNX+rkJEzNxft1NlRNoxu3+bsUHYpJ7PMhUveV0hbkLE1t3f1yZN5kVdTGqvDcwFWrrmpTa/ZYB3L7xtVZN/f
kkQoG38UeQZTcs8u1h0X+QI6h7iOokMBbEdaKep4FDx3c3hXm2eSMjWjVdrgupoAs+YZGu0nQPABJq83uk8r216Cp1KUsG2aqEWC
60xoELGtWXX0XwlPUDKIVnTIWnfmh4qMXgNHa/OIfnKXpGWjmQXtimDJJ3xFM0cgDFno0k3NnGPB67KuZe1ye9CGt7vc5M1+2sAX
HVAONxrMJ2+pr6wkygfx0b1TPySDRrubzoubQyecefeNtn8Tw6SX4k6N2Mw0Ye2j2wMQK5urk9aUYnifINQ4DSWkpxOY7h4Iu0zm
BX3/Z1yt93gkfF5r4iKoj1EsCMF6RcuWj8raCuzlB2BOxZ5YlQsb8SlcE4DXSLTw4G7M3KKLa7+iWNeR0+yLP2TuHSyUT+pGEBeY
HiI6v0cF2RqOKDq9w1netkwjii080NQr+TuYbKCEdsya412NZ5mQi/rR/Rc6IEnStYmeIoXqWz1OHsYLfXx6CaF+It/8QV6ykv4i
atPM4djhxacW5lRGoIggT0jDhR5+V/NlzA151nQ664+Hx/BZgTz9KafFUyNdJ3JXzAIF7x3vtALgVyXLd+8y7ArEvuiXxr4b1lt2
qQTQGJSLIgpyJQxxqAPteSoHIbHIkmcYiHiJsAVD+giyeQsOAE6BMFXUwLb+UxbUuk2UBKSpvhadt4AsAWIFLjrzzy8I72jrheFb
49GwGjVRtQW3r+VkCMrkYFi+ZT7j35KMCoPny9rkqI6zTco07Z1aylxcTX5Fn8ZkJH8QLRCieBRPJB+haek60wdmoLrwr4LLU6ri
h4IUNnrNg6db1ItuENW19bmNuVBjIXp23u8LY3TsRNl1z+CCOv2U0+Yx3w57+J/lSrnZPapKj2YXnWebwhAyb4+h+ED1I8rL6NV6
MVJr+tZfJii7EwcvO0B6ep2GCRHWD29+ygBO4Cb/HUTUwF4Tq5KBPJvnd1x4gs0AeqktNe+zpCHi/SDWEDYBzAXCSmriZt8LBMx1
7ogTBkczPnV9wddlISwsIL5hqxxl5atGtkhSHgPaIiETPbWp3+bpktHAlAouXSlOxjYgmVmgEAjszFAO4RBhk5+2QeNmglDHeAhK
pUGhGFkfE1fNuYjHYy28vQOW6rHBXvBqeBs16EPlWSFBZXq3O6P2793Io+oW7t7FqcVdMJ1WeUZRNqBN2TsD6GBndf2i9YFfn/rj
+LsWQ5k8cD5+mBiD+vwkZgUDuQaitLuJv2jREqRD/aCSYnCbSuMHAWj8hVBH04sBjp+MS+b0QUP/nJQgW12KOpp38Yh7iRsSDKet
pi9rMevt35QUt6G5riE8q75e1rYLclq139HylfV0ek8/RqsUhFq4QovJdXLovlNnTt1ssyXhoiPUpWIJ+zmekE1iileGGCdTlpQ6
MfzX3SF+i1cYF7cZXaR3T974rldCKuU1hjP74Sa2GSyRt2A3DNhAi2/idniWSTyzHshIavg86AcXokbtcqogYf6ah5Y0QEMGuCL3
X6Rsn00HSNJufb4kVM9ZH2qBUwi1whR6C7GWfs6FQaLVRZhSuwMJ5jnXweRAZjLG6yDYdWUaQZ2f4oOLYNoD2TwLXw829lRXd2ze
3fAgOaWOz5OdvMxNuwP6wth/jnVv0/i4RQcjn+TccP34O2Odb7jt/KE2ZixvzaQLPCOL1/jnfwoHd+Suaz+uwTvORY+gXWMvkYHU
KW3MWIdjGFPBQKkFAMxplffET5Fhl5TwkXgUau7AbEZnA1WcOx9tO0Lb7XCCyTSzRsd7W1HwcoHI27cPTr108ORing12zVrYVwRe
lboUbcgzUSQXRQDkZpDDw29obfyFVuycICs3THlsowRNodzXtH3uy+2ZXbvTfidsovn7f/kXyT7deD2dDASqqqIMpsSz/B7heZIO
7AkgyLIDyql/cSMLNdiuCjwwUc/Y/rMJxsdiqLbrCgZnGN01rL97mTFrKzl+8XDsFfHioSfCJf1WJvH6kRHylGcspB0LaF18xLie
fcN5msVWdpK1CFjulOLv73W9gQgfaIbCQl1T4K2UZexvM7gp3Jw8lBqPPZwOTg86G4jnzzOwfwVMp0h38pXlIKJ8f2E6xgLpgOzt
kDr6xAROKcUUvJaovsSi7SVDxKGEmhz5kd6t/1WF/k5Wwt7aQeNEKdjAwHQLw7Csl+jdlcxLaPYeASIlkCp/ns5YUevl2KsyD5rI
s8mo7d4eAWdsqFiCC2novDDBHom+UTGlo2HExr9Nq5YhgrUsdRNUB57DAvZl1kaB/RxqcPEBGmKX80psWUqoLwyyUIikvDcdfQb6
605LT90bMsmS0yqlIGhSlIHY575UGtPpo3XN3UDlTQPqBi8UTLNtURZxBVvSHTAgUhe1RIVYptBqUZxXXWV4vYYmyxpJPkfea5an
0xWIF5ZFrYspMAh+gW9eufsnFfOxCFfFq1IV1yPP5tj+sMPK0koGEb/aT419uDmq6nRct1WS5trNsmw0L8ZXaVkvf9Cbk+exIQRD
1rjKR/Moavy/Yy+Da8n5DR/dweHpguY0XjhZH1acq5ZRTxAh82ppG7TJp/YprBQWopbH018SWkPClfnKz1uWEseZMhyp1wm1dLYt
L+rwGqIUQnzP1Mg3PuTMX21gnkxpM4UugGoHbMuNAm6o9Tpo0ntXtnzu1k4PYgMn7YesRNZpw+xg==";
        private const string formDataHeaderName1 = "Content-Disposition";
        private const string formDataHeaderValue1 = "form-data; name=\"content1\"";
        private const string formData = @"Hello, George!!!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
sVdrpNKNLtzimyIgmwqw==";
        private const string formDataHeaderName2 = "Content-Disposition";
        private const string formDataHeaderValue2 = "form-data; name=\"content2\"; filename=\"filename\"";
        private const string formDataHeaderName3 = "Content-Transfer-Encoding";
        private const string formDataHeaderValue3 = "base64";
        private const string fileData = @"This is the content of the file.9dfeT7wZu+F8/Vs2svxG7Pq4Hn
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
1Hl/2AkCTHkSx3s8NDBayulw/ZegpBOsN8AQ==";
        private const string epilogue = @"This is the epilogue. It can be arbitrarily long and contain
newlines and other stuff.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
+FzqCv0X9XrBOclJZNILAkyBQEhssV+1Q==";

        #endregion

        [TestMethod]
        public void ShouldParseMultiPartFile()
        {
            Encoding encoding = Encoding.UTF8;
            byte[] binaryContent = encoding.GetBytes(getMultiPartContents());
            MemoryStream bodyStream = new MemoryStream(binaryContent);
            List<MultiPartSection> sections = new List<MultiPartSection>();

            StreamingMultiPartParser parser = new StreamingMultiPartParser(bodyStream, encoding, boundary);
            MemoryStream preambleStream = null;
            MemoryStream epilogueStream = null;
            parser.PreambleFound += (o, e) => preambleStream = e;
            parser.SectionFound += (o, e) => sections.Add(e);
            parser.EpilogueFound += (o, e) => epilogueStream = e;

            parser.Parse().Wait();

            using (StreamReader preambleReader = new StreamReader(preambleStream, encoding))
            {
                string parsedPreamble = preambleReader.ReadToEnd();
                Assert.AreEqual(preamble, parsedPreamble, "The preample was not read correctly.");
            }

            Assert.AreEqual(2, sections.Count, "Two sections should have been parsed from the body.");
            var section1 = sections[0];
            Assert.AreEqual(formDataHeaderValue1, section1.Headers[formDataHeaderName1], "The first header was not parsed.");
            using (StreamReader formDataReader = new StreamReader(section1.Content, encoding))
            {
                string parsedFormData = formDataReader.ReadToEnd();
                Assert.AreEqual(formData, parsedFormData, "The form data was not read correctly.");
            }

            var section2 = sections[1];
            Assert.AreEqual(formDataHeaderValue2, section2.Headers[formDataHeaderName2], "The second header was not parsed.");
            Assert.AreEqual(formDataHeaderValue3, section2.Headers[formDataHeaderName3], "The third header was not parsed.");
            using (StreamReader fileDataReader = new StreamReader(section2.Content, encoding))
            {
                string parsedFileData = fileDataReader.ReadToEnd();
                Assert.AreEqual(fileData, parsedFileData, "The file data was not read correctly.");
            }

            using (StreamReader epilogueReader = new StreamReader(epilogueStream, encoding))
            {
                string parsedEpilogue = epilogueReader.ReadToEnd();
                Assert.AreEqual(epilogue, parsedEpilogue, "The epilogue was not parsed.");
            }
        }

        private static string getMultiPartContents()
        {
            const string content = preamble + @"
--" + boundary + @"
" + formDataHeaderName1 + ": " + formDataHeaderValue1 + @"

" + formData + @"
--" + boundary + @"
" + formDataHeaderName2 + ": " + formDataHeaderValue2 + @"
" + formDataHeaderName3 + ": " + formDataHeaderValue3 + @"

" + fileData + @"
--" + boundary + @"--
" + epilogue;
            return content;
        }

        [TestMethod]
        public void ShouldHandleMultiPartMixedContent()
        {
            Encoding encoding = Encoding.UTF8;
            byte[] binaryContent = encoding.GetBytes(getMultiPartMixedContents());
            MemoryStream bodyStream = new MemoryStream(binaryContent);
            List<MultiPartSection> sections = new List<MultiPartSection>();

            StreamingMultiPartParser parser = new StreamingMultiPartParser(bodyStream, encoding, "AaB03x");
            parser.PreambleFound += (o, e) =>
            {
                string preamble = encoding.GetString(e.ToArray());
                Console.Out.WriteLine(preamble);
            };
            parser.SectionFound += (o, e) =>
            {
                MemoryStream stream = new MemoryStream();
                e.Content.CopyTo(stream);
                string section = encoding.GetString(stream.ToArray());
                Console.Out.WriteLine(section);
            };

            parser.Parse().Wait();
        }

        private static string getMultiPartMixedContents()
        {
            const string contents = @"--AaB03x
Content-Disposition: form-data; name=""submit-name""

Larry
--AaB03x
Content-Disposition: form-data; name=""files""
Content-Type: multipart/mixed; boundary=BbC04y

--BbC04y
Content-Disposition: file; filename=""file1.txt""
Content-Type: text/plain

... contents of file1.txt ...
--BbC04y
Content-Disposition: file; filename=""file2.gif""
Content-Type: image/gif
Content-Transfer-Encoding: binary

...contents of file2.gif...
--BbC04y--
--AaB03x--";
            return contents;
        }
    }
}
