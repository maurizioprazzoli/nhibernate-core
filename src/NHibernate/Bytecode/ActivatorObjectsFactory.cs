using System;

namespace NHibernate.Bytecode
{
	public class ActivatorObjectsFactory: IObjectsFactory
	{
        public object CreateInstance(System.Type type)
		{
            object instance = NHibernate.Cfg.Environment.BytecodeProvider.BytecodeProviderInterceptor.CreateInstance(type);
             if (instance == null)
             {
                 return Activator.CreateInstance(type);
             }
             return instance;
		}

		public object CreateInstance(System.Type type, bool nonPublic)
		{
            object instance = NHibernate.Cfg.Environment.BytecodeProvider.BytecodeProviderInterceptor.CreateInstance(type, nonPublic);
            if (instance == null)
            {
                return Activator.CreateInstance(type, nonPublic);
            }
            return instance;
		}

		public object CreateInstance(System.Type type, params object[] ctorArgs)
		{
            object instance = NHibernate.Cfg.Environment.BytecodeProvider.BytecodeProviderInterceptor.CreateInstance(type, ctorArgs);
            if (instance == null)
            {
                return Activator.CreateInstance(type, ctorArgs);
            }
            return instance;
		}
	}
}