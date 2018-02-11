﻿using JetBrains.Annotations;

namespace Tauron.Application.Common.BaseLayer.BusinessLayer
{
    [PublicAPI]
    public interface IBusinessRule : IRuleBase
    {
        void Action();
    }
}