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
namespace OdoyuleRules.Designer
{
    using System;
    using System.Linq.Expressions;

    public interface Binding<T>
        where T : class
    {
        Binding<T> When(Expression<Func<T, bool>> expression);

        Binding<T, TRight> Join<TRight>()
            where TRight : class;

        Binding<T> Then(Action<ThenConfigurator<T>> configureCallback);
    }

    public interface Binding<TLeft, TRight>
        where TLeft : class
        where TRight : class
    {
        Binding<TLeft, TRight> When(Expression<Func<TLeft, bool>> expression);
        Binding<TLeft, TRight> When(Expression<Func<TRight, bool>> expression);
        Binding<TLeft, TRight> When(Expression<Func<TLeft, TRight, bool>> expression);

        Binding<TLeft, TRight> Then(Action<ThenConfigurator<TLeft>> leftAction);
        Binding<TLeft, TRight> Then(Action<ThenConfigurator<TRight>> rightAction);
        Binding<TLeft, TRight> Then(Action<ThenConfigurator<TLeft,TRight>> configureCallback);
    }
}