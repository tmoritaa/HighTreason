using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;

namespace HighTreasonGame
{
    public class CardTemplateGenerator
    {
        public Dictionary<string, CardTemplate> CardTemplates {
            get;
            private set;
        }

        public CardTemplateGenerator(string cardInfoJson)
        {
            CardTemplates = new Dictionary<string, CardTemplate>();
            generateCardTemplates(cardInfoJson);
        }

        public List<CardTemplate> GetAllCardTemplates()
        {
            List<CardTemplate> cards = CardTemplates.Values.ToList();

            return CardTemplates.Values.ToList();
        }

        private void generateCardTemplates(string cardInfoJson)
        {
            var cardTemplateTypes =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(CardTemplateAttribute), true)
                where attributes != null && attributes.Length > 0
                select new { Type = t, Attributes = attributes.Cast<CardTemplateAttribute>() };

            JObject infoRoot = JObject.Parse(cardInfoJson);

            foreach (var type in cardTemplateTypes)
            {
                CardTemplate template = (CardTemplate)Activator.CreateInstance(type.Type);
                template.Init(infoRoot);
                CardTemplates.Add(template.Name, template);
            }
        }
    }
}
