using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighTreasonGame
{
    public interface IChoiceHandler
    {
        void ChooseCardUsage();
        AspectTrack ChooseAspectTrack(List<HTGameObject> choices);
        Jury.JuryAspect ChooseJuryAspects(List<HTGameObject> choices);
    }
}
