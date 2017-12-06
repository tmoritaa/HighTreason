using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json.Linq;
using HighTreasonGame.CardTemplates;

namespace HighTreasonGame
{
    public class CardTemplateGenerator
    {
        public Dictionary<string, CardTemplate> CardTemplates {
            get;
            private set;
        }

        // TODO: only necessary since generating cards. Should be deleted once enough cards are implemented.
        public JObject infoRoot;

        public CardTemplateGenerator(string cardInfoJson)
        {
            CardTemplates = new Dictionary<string, CardTemplate>();
            generateCardTemplates(cardInfoJson);
        }

        public List<CardTemplate> GetAllCardTemplates()
        {
            List<CardTemplate> cards = CardTemplates.Values.ToList();

            // TODO: only for now until we have enough cards.
            while (CardTemplates.Keys.Count < 45)
            {
                CardTemplate tmp1 = new JohnAstleyCardTemplate(infoRoot);
                tmp1.SetName(tmp1.Name + CardTemplates.Keys.Count.ToString().PadLeft(2, '0'));
                CardTemplates.Add(tmp1.Name, tmp1);

                CardTemplate tmp2 = new PurelyConstitutionalCardTemplate(infoRoot);
                tmp2.SetName(tmp2.Name + CardTemplates.Keys.Count.ToString().PadLeft(2, '0'));
                CardTemplates.Add(tmp2.Name, tmp2);
            }

            return CardTemplates.Values.ToList();
        }

        private void generateCardTemplates(string cardInfoJson)
        {
            infoRoot = JObject.Parse(cardInfoJson);

            addCardTemplate(new JohnAstleyCardTemplate(infoRoot));
            addCardTemplate(new PurelyConstitutionalCardTemplate(infoRoot));
            addCardTemplate(new CouncilExovedateCardTemplate(infoRoot));
            addCardTemplate(new WilliamTompkinsCardTemplate(infoRoot));
            addCardTemplate(new FatherVitalCardTemplate(infoRoot));
            addCardTemplate(new HaroldRossCardTemplate(infoRoot));
            addCardTemplate(new DoctorJamesCardTemplate(infoRoot));
            addCardTemplate(new ThomasMcKayCardTemplate(infoRoot));
            addCardTemplate(new JailersCardTemplate(infoRoot));
            addCardTemplate(new GeorgeHolmesYoungCardTemplate(infoRoot));
        }

        private void addCardTemplate(CardTemplate template)
        {
            CardTemplates.Add(template.Name, template);
        }
    }
}
