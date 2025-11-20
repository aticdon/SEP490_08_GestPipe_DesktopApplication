using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using System.Collections.Generic;
using System.Linq;

namespace GestPipePowerPonit.Services
{
    public class SlideInfoService
    {
        public List<int> GetSlidesWith3D(string pptxPath)
        {
            var result = new List<int>();
            using (PresentationDocument doc = PresentationDocument.Open(pptxPath, false))
            {
                var presPart = doc.PresentationPart;
                var slides = presPart.SlideParts.ToList();
                for (int i = 0; i < slides.Count; i++)
                {
                    var sp = slides[i];
                    bool found = false;
                    foreach (var pair in sp.Parts)
                    {
                        var p = pair.OpenXmlPart;
                        string ct = (p.ContentType ?? "").ToLower();
                        string uri = (p.Uri?.ToString() ?? "").ToLower();
                        if (ct.Contains("model/gltf") || ct.Contains("model3d") ||
                            uri.EndsWith(".glb") || uri.Contains("/model3d/"))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found) result.Add(i + 1);
                }
            }
            return result;
        }

        public List<string> GetSlideTitles(string pptxPath)
        {
            var titles = new List<string>();
            using (PresentationDocument presentationDocument = PresentationDocument.Open(pptxPath, false))
            {
                PresentationPart presentationPart = presentationDocument.PresentationPart;
                var slideIds = presentationPart.Presentation.SlideIdList.ChildElements;
                foreach (SlideId slideId in slideIds)
                {
                    SlidePart slidePart = (SlidePart)presentationPart.GetPartById(slideId.RelationshipId);
                    string title = null;
                    foreach (DocumentFormat.OpenXml.Presentation.Shape shape in slidePart.Slide.Descendants<DocumentFormat.OpenXml.Presentation.Shape>())
                    {
                        var ph = shape.NonVisualShapeProperties?.ApplicationNonVisualDrawingProperties?.GetFirstChild<PlaceholderShape>();
                        if (ph != null && (ph.Type == PlaceholderValues.Title || ph.Type == PlaceholderValues.CenteredTitle))
                        {
                            title = string.Join(" ", shape.TextBody.Descendants<DocumentFormat.OpenXml.Drawing.Text>().Select(t => t.Text));
                            break;
                        }
                    }
                    titles.Add(title ?? "");
                }
            }
            return titles;
        }
    }
}