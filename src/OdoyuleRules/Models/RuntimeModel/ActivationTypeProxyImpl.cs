// Copyright 2011 Chris Patterson
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace OdoyuleRules.Models.RuntimeModel
{
    using System;

    class ActivationTypeProxyImpl<T> :
        ActivationTypeProxy
        where T : class
    {
        public FactHandle Activate(RulesEngine rulesEngine,
            ActivationContext baseContext,
            FactCache factCache,
            object obj)
        {
            var fact = obj as T;
            if (fact == null)
                throw new ArgumentException("The argument could not be cast to " + typeof (T).Name, "obj");

            ActivationContext<T> context = baseContext.CreateContext(fact);

            rulesEngine.Activate(context);

            return factCache.Add(context);
        }
    }
}