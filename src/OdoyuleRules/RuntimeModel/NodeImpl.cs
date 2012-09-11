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
namespace OdoyuleRules.RuntimeModel
{
    using System.Linq;


    public abstract class NodeImpl<T>
        where T : class
    {
        readonly int _id;

        readonly ActivationList<T> _successors;

        protected NodeImpl(int id)
        {
            _id = id;
            _successors = new ActivationList<T>();
        }

        public int Id
        {
            get { return _id; }
        }

        public virtual void Activate(ActivationContext<T> context)
        {
            _successors.All(activation => activation.Activate(context));
        }

        public bool Successors(RuntimeModelVisitor visitor)
        {
            return Enumerable.All(_successors, activation => activation.Accept(visitor));
        }

        public void AddActivation(Activation<T> activation)
        {
            _successors.Add(activation);
        }

        public void RemoveActivation(Activation<T> activation)
        {
            _successors.Remove(activation);
        }
    }
}