using Lime.Messaging.Contents;
using Lime.Protocol;
using PratiqueBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PratiqueBot.Factory
{
    public class DocumentService
    {
        public MediaLink CreateMediaLink(string title, string text, Uri url, string mediatype)
        {
            var type = mediatype.Split('/')[0];
            var subtype = mediatype.Split('/')[1];
            MediaLink mediaLink = new MediaLink
            {
                Title = title,
                Text = text,
                PreviewUri = url,
                Uri = url,
                Size = 1,
                Type = new MediaType(type, subtype)

            };
            return mediaLink;
        }

        public DocumentSelectOption[] CreateDocumentSelectOptions(List<CarrosselOptions> options)
        {
            DocumentSelectOption[] opts = new DocumentSelectOption[options.Count];
            int i = 0;
            foreach (var option in options)
            {
                if (option.label.GetType() == typeof(WebLink))
                {
                    opts[i] = new DocumentSelectOption();
                    opts[i].Label = new DocumentContainer { Value = (WebLink)option.label };
                    opts[i].Value = new DocumentContainer { Value = option.value.ToString() };
                    i++;
                }
                else
                {

                    opts[i] = new DocumentSelectOption();
                    opts[i].Label = new DocumentContainer { Value = new PlainText { Text = option.label.ToString() } };
                    opts[i].Value = new DocumentContainer { Value = new PlainText { Text = option.value.ToString() } };
                    i++;

                }


            }


            return opts;

        }

        public WebLink CreateWebLink(string title, string text, Uri url, string mediatype)
        {
            WebLink webLink = new WebLink
            {
                Title = title,
                Text = text,
                Uri = url,
                PreviewUri = url,
                PreviewType = MediaType.Parse(mediatype)
            };
            return webLink;
        }

        public Select CreateSimpleSelect(SelectScope scope, string text, List<string> options)
        {
            Select select = new Select { Scope = scope, Text = text };
            SelectOption[] selectOptions = new SelectOption[options.Count];
            int count = 0;
            foreach (var option in options)
            {
                selectOptions[count] = new SelectOption { Text = options[count], Value = options[count] };
                count++;
            }
            select.Options = selectOptions;
            return select;
        }

        public DocumentCollection CreateCarrossel(List<CarrosselCard> cards)
        {
            var docCollection = new DocumentCollection();
            docCollection.Items = new DocumentSelect[cards.Count];
            docCollection.ItemType = DocumentSelect.MediaType;
            int i = 0;
            foreach (var card in cards)
            {
                var doc = new DocumentSelect();
                doc.Header = new DocumentContainer();
                doc.Header.Value = card.CardMediaHeader;
                doc.Options = CreateDocumentSelectOptions(card.options);
                docCollection.Items[i] = doc;
                i++;
            }

            return docCollection;

        }
    }
}
