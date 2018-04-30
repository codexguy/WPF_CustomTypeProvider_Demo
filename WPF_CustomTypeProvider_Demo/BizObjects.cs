/***********************************************************************
Copyright 2018 CodeX Enterprises LLC

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Major Changes:
04/2018    1.0     Initial release (Joel Champagne)
***********************************************************************/
using System.Threading;

/// <summary>
/// We assume these Person entities have two fixed properties (ID, Name). Name will come from json file, ID will be system-assigned. All additional properties will derive dynamically from json file source.
/// </summary>

namespace WPF_CustomTypeProvider_Demo.NoInherit
{
    public class Person
    {
        private static int _id = 0;

        public Person()
        {
            ID = Interlocked.Increment(ref _id);
        }

        public int ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }
}

namespace WPF_CustomTypeProvider_Demo.Inherit
{
    public class Person : TypeProviderBase
    {
        private static int _id = 0;

        public Person()
        {
            ID = Interlocked.Increment(ref _id);
        }

        public int ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
    }
}
