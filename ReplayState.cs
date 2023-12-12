using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaySystem
{
    public abstract class ReplayState
    {
        public virtual void Start()
        {

        }

        public virtual void OnFixedUpdate()
        {

        }

        public virtual void End()
        {

        }

        public virtual void OnUpdate()
        {

        }
    }
}
