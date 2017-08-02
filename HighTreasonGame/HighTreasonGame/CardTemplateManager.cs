using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HighTreasonGame.CardTemplates;

namespace HighTreasonGame
{
    public class CardTemplateManager
    {
        private static CardTemplateManager instance = null;
        public static CardTemplateManager Instance {
            get {
                if (instance == null)
                {
                    instance = new CardTemplateManager();
                }

                return instance;
            }
        }

        public Dictionary<string, CardTemplate> CardTemplates {
            get;
            private set;
        }
        
        public void Test()
        {
            CardTemplates["John W. Astley"].SummationEvents[0](0, new TestChoiceHandler());
        }

        private CardTemplateManager()
        {
            CardTemplates = new Dictionary<string, CardTemplate>();
            generateCardTemplates();
        }

        private void generateCardTemplates()
        {
            addCardTemplate(new JohnAstleyCardTemplate());
        }

        private void addCardTemplate(CardTemplate template)
        {
            CardTemplates.Add(template.Name, template);
        }
    }
}
