namespace NHibernate.Bytecode
{
    public interface IInjectableBytecodeProviderInterceptorClass
	{
        void SetBytecodeProviderInterceptorClass(string typeAssemblyQualifiedName);
        void SetBytecodeProviderInterceptorClass(System.Type type);
	}
}