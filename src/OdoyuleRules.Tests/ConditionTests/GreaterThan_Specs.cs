namespace OdoyuleRules.Tests.ConditionTests
{
    using System;
    using Configuration;
    using Models.SemanticModel;
    using NUnit.Framework;

    [TestFixture]
    public class Conditions_using_greater_than
    {
        [Test]
        public void Should_match_greater_values()
        {
            _result = null;

            using (Session session = _engine.CreateSession())
            {
                session.Add(new Order { Amount = 10001.0m });
                session.Run();
            }

            Assert.IsNotNull(_result);
        }

        [Test]
        public void Should_not_match_equal_values()
        {
            _result = null;

            using (Session session = _engine.CreateSession())
            {
                session.Add(new Order {Amount = 10000.0m});
                session.Run();
            }

            Assert.IsNull(_result);
        }

        [Test]
        public void Should_not_match_less_than_values()
        {
            _result = null;

            using (Session session = _engine.CreateSession())
            {
                session.Add(new Order {Amount = 9999.9m});
                session.Run();
            }

            Assert.IsNull(_result);
        }

        Order _result;
        RulesEngine _engine;

        [TestFixtureSetUp]
        public void Define_rule()
        {
            var conditions = new RuleCondition[]
                {
                    Conditions.GreaterThan((Order x) => x.Amount, 10000.0m),
                };

            var consequence = new DelegateConsequence<Order>((session,x) => { _result = x; });
            var consequences = new RuleConsequence[]
                {
                    consequence,
                };

            Rule rule = new OdoyuleRule("RuleA", conditions, consequences);

            _engine = RulesEngineFactory.New(x => x.Add(rule));
        }

        class Order
        {
            public decimal Amount { get; set; }
        }
    }
}