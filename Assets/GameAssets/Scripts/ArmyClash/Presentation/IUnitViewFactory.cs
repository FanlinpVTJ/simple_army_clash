using System;
using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Presentation
{
    public interface IUnitViewFactory : IDisposable
    {
        IUnitView Create(UnitState state);
        void Release(IUnitView view);
    }
}
