﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetrExamples
{

    partial class PartialClass
    {
        partial void method();
    }

    partial class PartialClass
    {
        partial void method()
        {
            classFromAnotherSyntaxTree.method();
        }
    }

    class PropertyClass
    {

        int d_field1;
        public int d_pro1
        {
            get
            {
                B.PublicMethod2_B();//*** for CBO is the same that lower// that's why counted 1 per d_pro1
                return d_field1;
            }
            set
            {
                C.PublicMethod1_C();
                C.PublicMethod2_B();//*** for CBO counted as method B.PublicMethod2_B(), which was present earlier // that's why counted 1 per d_pro1
                d_field1 = value;
            }
        }
    }

    //CBO == 2, PublicMethod1_B() called in different places
    class DiffPlaceSameExternalMethodCall
    {
        public void op1()
        {
            B.PublicMethod1_B();
        }
        public void op2()
        {
            B.PublicMethod1_B();
        }
    }

    //CBO == 0, no count for self calling
    class SingleClass
    {
        public void s_op1()
        {
            s_op2();
        }
        public void s_op2()
        {

        }
    }

    class ParentChildExternalMethodCalled
    {
        //CBO == 2, because B.PublicMethod1_B() and C.PublicMethod1_B() are the same
        public void e_op1()
        {
            B.PublicMethod1_B();
            C.PublicMethod1_B();

            C.PublicMethod1_C();
        }
    }

    class A
    {

    }

    class B : A
    {
        static public void PublicMethod1_B() { }
        static public void PublicMethod2_B() { }

        protected String _field1;

        protected B() { }
        private B(Int32 a)
        {
            _field1 = a.ToString();
        }

        protected B(String a)
        {
            _field1 = a.ToString();
        }

        public B(Int64 a)
        {
            _field1 = a.ToString();
        }

        private void PrivateMethod1_B() { }
        virtual protected void ProtectedMethod1_B() { }

        public void MethodToBeOverwritten() { }

    }

    class B2 : A
    {

    }

    class B3 : A
    {

    }

    class B2Children1 : B2
    {

    }

    class B2Children2 : B2
    {

    }

    class C : B
    {
        class InternalDefinedClass1
        {

        }


        private C(Int32 a)
        {
            _field1 = a.ToString();
        }

        protected C(String a)
        {
            _field1 = a.ToString();
        }

        public C(Int64 a)
        {
            _field1 = a.ToString();
        }


        private void PrivateMethod1_C() { }
        protected void ProtectedMethod1_C() { }
        static public void PublicMethod1_C() { }
        public void PublicMethod2_C() { }

        public new static void PublicMethod1_B() { }

        protected override void ProtectedMethod1_B()
        {
            base.ProtectedMethod1_B();
        }

        public new void MethodToBeOverwritten() { }
        public void anotherMethod() { }
    }
}
