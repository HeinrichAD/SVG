using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Svg.UnitTests
{

    /// <summary>
    /// Test Class of rendering SVGs on a printer graphic.
    /// Based on Issue 214.
    /// </summary>
    /// <remarks>
    /// Special:
    /// The graphic from a printer could have diffent graphic.DpiY values.
    /// Text elements use graphic.DpiY to get their render position.
    /// 
    /// Test use the following embedded resources:
    ///   - Issue214_PrinterGraphicRendering\Svg2Print.svg
    /// </remarks>
    [TestClass]
    public class PrinterGraphicRenderingTest : SvgTestHelper
    {
        protected override string TestResource { get { return GetFullResourceString("Issue214_PrinterGraphicRendering.Svg2Print.svg"); } }
        protected override int ExpectedSize { get { return 8300; } }

        private bool useDefaultPrinter { get { return true; } }
        private bool doReallyPrint { get { return false; } }


        [TestMethod]
        public void TestDefaultSvgRendering()
        {
            LoadSvg(GetXMLDocFromResource());
        }
        

        [TestMethod]
        public void TestPrinterGraphicRenderingBySvgRendererWrapper()
        {
            // Output quality is quite good.
            TestPrinterGraphicRendering(printDoc_PrintPageBySvgRendererWrapper);
        }


        private void printDoc_PrintPageBySvgRendererWrapper(object sender, PrintPageEventArgs e)
        {
            const float DefaultGraphicDpiY = 96f;

            //PrintDocument printDoc = (PrintDocument)sender;
            SvgDocument svgDoc = OpenSvg(GetXMLDocFromResource());
            SvgRendererFixDpiY renderer = new SvgRendererFixDpiY(e.Graphics, DefaultGraphicDpiY);
            svgDoc.Draw(renderer);
        }


        private class SvgRendererFixDpiY : SvgRenderer, ISvgRenderer
        {
            public SvgRendererFixDpiY(Graphics graphic, float fixDpiY) : base(graphic)
            {
                if (fixDpiY <= 0f)
                    throw new ArgumentException("Fix DPI Y size must grater then zero.", nameof(fixDpiY));
                dpiY = fixDpiY;
            }


            private float dpiY;
            public new float DpiY
            {
                get { return dpiY; }
            }
        }


        [TestMethod]
        public void TestPrinterGraphicRenderingByImagePrinting()
        {
            // Output quality is not so good.
            TestPrinterGraphicRendering(printDoc_PrintPageByImagePrinting);
        }


        private void printDoc_PrintPageByImagePrinting(object sender, PrintPageEventArgs e)
        {
            SvgDocument svgDoc = OpenSvg(GetXMLDocFromResource());
            var img = svgDoc.Draw();
            e.Graphics.DrawImageUnscaled(img, Point.Empty);
        }


        [TestMethod]
        public void TestPrinterGraphicRenderingByGraphicDrawing()
        {
            // Output text quality is quite bad.
            TestPrinterGraphicRendering(printDoc_PrintPageByGraphicDrawing);
        }


        private void printDoc_PrintPageByGraphicDrawing(object sender, PrintPageEventArgs e)
        {
            PrintDocument printDoc = (PrintDocument)sender;
            using (var img = new Bitmap(printDoc.DefaultPageSettings.PaperSize.Width, printDoc.DefaultPageSettings.PaperSize.Height)) // Not necessary if you use Control.CreateGraphics().
            using (Graphics graphics = Graphics.FromImage(img)) // Control.CreateGraphics()
            {
                SvgDocument svgDoc = OpenSvg(GetXMLDocFromResource());
                svgDoc.Draw(graphics);
                e.Graphics.DrawImageUnscaled(new Bitmap(img.Width, img.Height, graphics), Point.Empty);
            }
        }


        private void TestPrinterGraphicRendering(PrintPageEventHandler svgPrintHandler)
        {
            // Sample code based on Issue 214. Thanks to butlnor.
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += svgPrintHandler;

            bool doPrint = useDefaultPrinter;
            if (!useDefaultPrinter)
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDoc;

                if (printDialog.ShowDialog(null) == DialogResult.OK)
                {
                    doPrint = true;
                }
            }

            if (doPrint && doReallyPrint)
                printDoc.Print();
        }
    }
}
