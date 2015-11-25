﻿using System;
using System.Collections;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.BytecodeInterceptor
{
    [Serializable]
    public class EmptyBytecodeProviderInterceptor : IBytecodeProviderInterceptor
    {
        public virtual object CreateInstance(System.Type type)
        {
            return null;
        }

        public virtual object CreateInstance(System.Type type, bool nonPublic)
        {
            return null;
        }

        public virtual object CreateInstance(System.Type type, object[] ctorArgs)
        {
            return null;
        }

        public virtual object CreateProxyInstance(System.Type proxyType)
        {
            return null;
        }

        public virtual object CreateProxyInstance(System.Type proxyType, object[] args)
        {
            return null;
        }

        public virtual void ProxyTypeCreated(System.Type proxyType)
        {

        }
    }
}