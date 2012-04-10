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
    using System.Collections;
    using System.Linq.Expressions;
    using System.Reflection;
    using Conditionals;
    using Configuration.RuntimeModelConfigurators;
    using Models.RuntimeModel;
    using Util;

    public static class RuntimeConfiguratorExtensions
    {
        public static PropertyNode<T, TProperty> Property<T, TProperty>(
            this RuntimeConfigurator configurator, Expression<Func<T, TProperty>> propertyExpression)
            where T : class
        {
            PropertyInfo propertyInfo = propertyExpression.GetPropertyInfo();

            PropertyNode<T, TProperty> propertyNode =
                configurator.CreateNode(id => new PropertyNode<T, TProperty>(id, propertyInfo));

            return propertyNode;
        }

        public static PropertyNode<T, TProperty> Property<T, TProperty>(
            this RuntimeConfigurator configurator, PropertyInfo propertyInfo)
            where T : class
        {
            PropertyNode<T, TProperty> propertyNode =
                configurator.CreateNode(id => new PropertyNode<T, TProperty>(id, propertyInfo));

            return propertyNode;
        }

        public static PropertyNode<T, TProperty> Property<T, TProperty>(
            this RuntimeConfigurator configurator, PropertyInfo propertyInfo, Action<T, Action<TProperty>> propertyMatch)
            where T : class
        {
            PropertyNode<T, TProperty> propertyNode =
                configurator.CreateNode(id => new PropertyNode<T, TProperty>(id, propertyInfo, propertyMatch));

            return propertyNode;
        }

        public static EqualNode<T, TProperty> Equal<T, TProperty>(this RuntimeConfigurator configurator)
            where T : class
        {
            EqualNode<T, TProperty> propertyNode =
                configurator.CreateNode(id => new EqualNode<T, TProperty>(id, configurator));

            return propertyNode;
        }


        public static ConstantNode<T> Constant<T>(this RuntimeConfigurator configurator)
            where T : class
        {
            return configurator.CreateNode(id => new ConstantNode<T>(id));
        }

        public static ConditionNode<T> Condition<T>(
            this RuntimeConfigurator configurator, Predicate<T> condition)
            where T : class
        {
            ConditionNode<T> conditionNode = configurator.CreateNode(
                id => new ConditionNode<T>(id, (value, next) =>
                    {
                        if (condition(value))
                            next();
                    }));

            return conditionNode;
        }

        public static ConditionNode<Token<T1, T2>> Condition<T1, T2>(
            this RuntimeConfigurator configurator, Predicate<T2> condition)
            where T1 : class
        {
            ConditionNode<Token<T1, T2>> conditionNode = configurator.CreateNode(
                id => new ConditionNode<Token<T1, T2>>(id, (value, next) =>
                    {
                        if (condition(value.Item2))
                            next();
                    }));

            return conditionNode;
        }

        public static AlphaNode<T> Alpha<T>(this RuntimeConfigurator configurator)
            where T : class
        {
            return configurator.CreateNode(id => new AlphaNode<T>(id));
        }

        public static AlphaNode<Token<T1, T2>> Alpha<T1, T2>(
            this RuntimeConfigurator configurator)
            where T1 : class
        {
            return configurator.CreateNode(id => new AlphaNode<Token<T1, T2>>(id));
        }

        public static LeftJoinNode<T1, T2> Left<T1, T2>(
            this RuntimeConfigurator configurator, RightActivation<T1> rightActivation)
            where T1 : class
        {
            return configurator.CreateNode(id => new LeftJoinNode<T1, T2>(id, rightActivation));
        }

        public static JoinNode<T> Join<T>(this RuntimeConfigurator configurator,
                                          RightActivation<T> rightActivation)
            where T : class
        {
            return configurator.CreateNode(id => new JoinNode<T>(id, rightActivation));
        }

        public static OuterJoinNode<T1,T2> Outer<T1,T2>(this RuntimeConfigurator configurator,
            RightActivation<T2> rightActivation)
            where T1 : class
            where T2: class
        {
            return configurator.CreateNode(id => new OuterJoinNode<T1, T2>(id, rightActivation));
        }


        public static DelegateProductionNode<T> Delegate<T>(this RuntimeConfigurator configurator,
                                                            Action<Session, T> callback)
            where T : class
        {
            return configurator.CreateNode(id => new DelegateProductionNode<T>(id, callback));
        }

        public static AddFactProductionNode<T,TFact> AddFact<T,TFact>(this RuntimeConfigurator configurator,
                                                            Func<T, TFact> factFactory)
            where T : class
            where TFact : class
        {
            return configurator.CreateNode(id => new AddFactProductionNode<T,TFact>(id, factFactory));
        }

        public static CompareNode<T, TProperty> Compare<T, TProperty>(this RuntimeConfigurator configurator,
                                                                      Comparator<TProperty, TProperty> comparator,
                                                                      TProperty value)
            where T : class
        {
            Value<TProperty> rightValue = Conditional.Constant(value);
            TokenValueFactory<T, TProperty> tokenValue = Conditional.Property<T, TProperty>();

            return configurator.CreateNode(id => new CompareNode<T, TProperty>(id, tokenValue, comparator, rightValue));
        }

        public static CompareNode<T, TProperty> Compare<T, TProperty>(this RuntimeConfigurator configurator,
                                                                      Comparator<TProperty, TProperty> comparator,
                                                                      Value<TProperty> value)
            where T : class
        {
            TokenValueFactory<T, TProperty> tokenValue = Conditional.Property<T, TProperty>();

            return configurator.CreateNode(id => new CompareNode<T, TProperty>(id, tokenValue, comparator, value));
        }

        public static CompareNode<T, TProperty> GreaterThan<T, TProperty>(this RuntimeConfigurator configurator,
                                                                          TProperty value)
            where T : class
            where TProperty : IComparable<TProperty>
        {
            Value<TProperty> rightValue = Conditional.Constant(value);
            TokenValueFactory<T, TProperty> tokenValue = Conditional.Property<T, TProperty>();

            var comparator = new GreaterThanValueComparator<TProperty>();

            return configurator.CreateNode(id => new CompareNode<T, TProperty>(id, tokenValue, comparator, rightValue));
        }

        public static CompareNode<T, TProperty> GreaterThanOrEqual<T, TProperty>(this RuntimeConfigurator configurator,
                                                                                 TProperty value)
            where T : class
            where TProperty : IComparable<TProperty>
        {
            Value<TProperty> rightValue = Conditional.Constant(value);
            TokenValueFactory<T, TProperty> tokenValue = Conditional.Property<T, TProperty>();

            var comparator = new GreaterThanOrEqualValueComparator<TProperty>();

            return configurator.CreateNode(id => new CompareNode<T, TProperty>(id, tokenValue, comparator, rightValue));
        }

        public static CompareNode<T, TProperty> LessThan<T, TProperty>(this RuntimeConfigurator configurator,
                                                                       TProperty value)
            where T : class
            where TProperty : IComparable<TProperty>
        {
            Value<TProperty> rightValue = Conditional.Constant(value);
            TokenValueFactory<T, TProperty> tokenValue = Conditional.Property<T, TProperty>();

            var comparator = new LessThanValueComparator<TProperty>();

            return configurator.CreateNode(id => new CompareNode<T, TProperty>(id, tokenValue, comparator, rightValue));
        }

        public static CompareNode<T, TProperty> LessThanOrEqual<T, TProperty>(this RuntimeConfigurator configurator,
                                                                              TProperty value)
            where T : class
            where TProperty : IComparable<TProperty>
        {
            Value<TProperty> rightValue = Conditional.Constant(value);
            TokenValueFactory<T, TProperty> tokenValue = Conditional.Property<T, TProperty>();

            var comparator = new LessThanOrEqualValueComparator<TProperty>();

            return configurator.CreateNode(id => new CompareNode<T, TProperty>(id, tokenValue, comparator, rightValue));
        }

        public static NotNullNode<T, TProperty> NotNull<T, TProperty>(this RuntimeConfigurator configurator)
            where T : class
            where TProperty : class
        {
            TokenValueFactory<T, TProperty> tokenValue = Conditional.Property<T, TProperty>();

            return configurator.CreateNode(id => new NotNullNode<T, TProperty>(id, tokenValue));
        }

        public static ExistsNode<T, TProperty> Exists<T, TProperty>(this RuntimeConfigurator configurator)
            where T : class
            where TProperty : class, IEnumerable
        {
            TokenValueFactory<T, TProperty> tokenValue = Conditional.Property<T, TProperty>();

            return configurator.CreateNode(id => new ExistsNode<T, TProperty>(id, tokenValue));
        }

        public static EachNode<T, TProperty, TElement> Each<T, TProperty, TElement>(
            this RuntimeConfigurator configurator,
            Action<TProperty, Action<TElement, int>> elementMatch)
            where T : class
            where TProperty : class, IEnumerable
        {
            TokenValueFactory<T, TProperty> tokenValue = Conditional.Property<T, TProperty>();

            return configurator.CreateNode(id => new EachNode<T, TProperty, TElement>(id, tokenValue, elementMatch));
        }
    }
}