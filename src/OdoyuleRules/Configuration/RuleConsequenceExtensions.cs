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
namespace OdoyuleRules
{
    using System;
    using Designer;
    using Models.RuntimeModel;
    using Models.SemanticModel;

    public static class RuleConsequenceExtensions
    {
        public static void Add<T, TFact>(this Binding<T> binding, Func<T, TFact> factFactory)
            where T : class
            where TFact : class
        {
            binding.Then(then =>
                {
                    AddFactConsequence<T, TFact> consequence = Consequences.Add(factFactory);

                    var consequenceConfigurator = new RuleConsequenceConfiguratorImpl<T>(consequence);

                    then.AddConfigurator(consequenceConfigurator);
                });
        }

        public static void Add<T, TFact>(this Binding<T> binding, Func<TFact> factFactory)
            where T : class
            where TFact : class
        {
            binding.Then(then =>
                {
                    AddFactConsequence<T, TFact> consequence = Consequences.Add<T, TFact>(x => factFactory());

                    var consequenceConfigurator = new RuleConsequenceConfiguratorImpl<T>(consequence);

                    then.AddConfigurator(consequenceConfigurator);
                });
        }


        public static Binding<TLeft, TRight> Add<TLeft, TRight, T>(this Binding<TLeft, TRight> binding,
            Func<T> factFactory)
            where TLeft : class
            where TRight : class
            where T : class
        {
            binding.Then((ThenConfigurator<TLeft, TRight> then) =>
                {
                    AddFactConsequence<Token<TLeft, TRight>, T> consequence =
                        Consequences.Add((Token<TLeft, TRight> t) => factFactory());

                    var consequenceConfigurator = new RuleConsequenceConfiguratorImpl<Token<TLeft, TRight>>(consequence);

                    then.AddConfigurator(consequenceConfigurator);
                });

            return binding;
        }

        public static void Delegate<T>(this ThenConfigurator<T> configurator, Action<T> callback)
            where T : class
        {
            DelegateConsequence<T> consequence = Consequences.Delegate<T>((session, fact) => callback(fact));

            var consequenceConfigurator = new RuleConsequenceConfiguratorImpl<T>(consequence);

            configurator.AddConfigurator(consequenceConfigurator);
        }
    }
}