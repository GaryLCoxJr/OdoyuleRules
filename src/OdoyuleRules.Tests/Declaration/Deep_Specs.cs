﻿namespace OdoyuleRules.Tests.Declaration
{
    using System;
    using System.Linq;
    using Configuration;
    using Models.SemanticModel;
    using NUnit.Framework;
    using Visualization;
    using Visualizer;

    [TestFixture]
    public class Going_deep_into_the_object_graph
    {
        [Test]
        public void Should_compile_and_execute()
        {
            _result = null;

            RulesEngine rulesEngine = RulesEngineFactory.New(x => { x.Add(_rule); });

            using (Session session = rulesEngine.CreateSession())
            {
                session.Add(new Order { Name = "JOE", Amount = 10001.0m , Purchaser = new Customer{AccountNumber = "DIRT"}});
                session.Run();
            }

            Assert.IsNotNull(_result);
        }

        [Test]
        public void Should_not_activate_for_only_one_side()
        {
            _result = null;

            RulesEngine rulesEngine = RulesEngineFactory.New(x => { x.Add(_rule); });

            using (Session session = rulesEngine.CreateSession())
            {
                session.Add(new Order { Name = "JOE", Amount = 9999.0m });
                session.Run();
            }

            Assert.IsNull(_result);
        }

        [Test]
        public void Should_not_activate_for_only_other_side()
        {
            _result = null;

            RulesEngine rulesEngine = RulesEngineFactory.New(x => { x.Add(_rule); });

            using (Session session = rulesEngine.CreateSession())
            {
                session.Add(new Order { Name = "MAMA", Amount = 10001.0m });
                session.Run();
            }

            Assert.IsNull(_result);
        }

        [Test]
        public void Should_have_the_proper_condition_count()
        {
            Assert.AreEqual(3, _rule.Conditions.Count());
        }

        [Test]
        public void Should_have_the_proper_consequence_count()
        {
            Assert.AreEqual(2, _rule.Consequences.Count());
        }

        [Test]
        [Explicit]
        public void Show_me_the_goods()
        {
            RulesEngine rulesEngine = RulesEngineFactory.New(x =>
            {
                x.Add(_rule);
                x.Add(_rule2);
            });

            rulesEngine.ShowVisualizer();
        }


        Order _result;
        Order _resultB;
        Rule _rule;
        Rule _rule2;

        [TestFixtureSetUp]
        public void Define_rule()
        {
            var conditions = new RuleCondition[]
                {
                    Conditions.Equal((Order x) => x.Name, "JOE"),
                    Conditions.GreaterThan((Order x) => x.Amount, 10000.0m),
                    Conditions.Equal((Order x) => x.Purchaser.AccountNumber, "DIRT"),
                };

            var consequence = new DelegateConsequence<Order>((Session session, Order x) => { _result = x; });
            var consequence1 = new DelegateConsequence<Order>((Session session, Order x) => { _resultB = x; });
            var consequences = new RuleConsequence[]
                {
                    consequence,
                    consequence1,
                };

            _rule = new OdoyuleRule("RuleA", conditions, consequences);
            _rule2 = new OdoyuleRule("RuleB", conditions, consequences);
        }

        class Customer
        {
            public string AccountNumber { get; set; }
            public string ContactNumber { get; set; }
        }

        class Order
        {
            public string Name { get; set; }
            public decimal Amount { get; set; }
            public Customer Purchaser { get; set; }
        }

        class Account
        {
            public string Name { get; set; }
            public decimal CreditLimit { get; set; }
        }
    }
}
