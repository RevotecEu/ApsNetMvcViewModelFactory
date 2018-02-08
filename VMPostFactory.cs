using Revo.Utilities;
using System;
using System.Web.Mvc;

namespace Revo.VMFactory
{
    public interface IResponse
    {
        bool IsOk { get; set; }
    }

    public class ResponsePost : IResponse
    {
        public ResponsePost(bool isOk)
        {
            IsOk = isOk;
        }

        public bool IsOk   { get;set; }
    }

    /// <summary>
    /// Interface of the factory to build views
    /// Dont pass a controller as TInput itself to the view factory to keep it loose coupled
    /// got the basic idea here: http://benfoster.io/blog/using-the-view-factory-pattern-in-aspnet-mvc
    /// </summary>
    public interface IVMPostFactory
    {
        /// <summary>
        /// Creates a view without parameter
        /// <see cref="DBoard.Utilities.VMFactory.VMFactoryBindings.RegisterBindings(Ninject.IKernel)"/>
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <returns></returns>
        IResponse PostView<TView>(TView view);

        /// <summary>
        /// Creates view with paramer of type TInput
        /// <see cref="DBoard.Utilities.VMFactory.VMFactoryBindings.RegisterBindings(Ninject.IKernel)"/>
        /// </summary>
        /// <typeparam name="TInput">parameter type that is passed to the factory</typeparam>
        /// <typeparam name="TView">type of view to build</typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        IResponse ProcessPost<TView, TInput>(TView view,TInput input);
    }

    public class VMDefaultPostFactory<TContext> : IVMPostFactory
    {
        /// <summary>
        /// Ctor of factory, Ninject injection DI can also be used
        /// IoTContext is the db context used when built
        /// </summary>
        /// <param name="context"></param>
        public VMDefaultPostFactory(TContext context)
        {
            _Context = context;
        }

        /// <summary>
        /// Creates a view of type TView without parameter. Usage:
        /// <para>1. Define VMExampleData  -> model class, passed to view</para>
        /// <para>2. Define VMExampleDataBuilder -> how to populate model class</para>
        /// <para>3. Bind IVMPostFactory to concrete  VMExampleDataBuilder VMExampleData in VMFactoryBindings</para>
        /// <para>VMFactoryBindings.RegisterBindings: register bindings there. VMBuilders.cs File for simple builders</para>
        /// <see cref="DBoard.Utilities.VMFactory.VMFactoryBindings.RegisterBindings(Ninject.IKernel)"/>
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <returns></returns>
        public IResponse PostView<TView>(TView view)
        {
            var processor = DependencyResolver.Current.GetService<IVMProcess<TContext,TView>>();

            if (processor != null) return processor.Process(_Context,view);
            RevTrace.TE("couldnt resolve");

            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a view of type TView with parameter of type TInput. Usage:
        /// <para>1. Define VMExampleData  -> model class, passed to view</para>
        /// <para>2. Define VMExampleDataBuilder -> how to populate model class</para>
        /// <para>3. Bind IVMPostFactory to concrete  VMExampleDataBuilder VMExampleData in VMFactoryBindings</para>
        /// <para>VMFactoryBindings.RegisterBindings -> register bindings there. VMBuilders.cs -> File for simple builders</para>
        /// <see cref="DBoard.Utilities.VMFactory.VMFactoryBindings.RegisterBindings(Ninject.IKernel)"/>
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <returns></returns>
        public IResponse ProcessPost<TView, TInput>(TView view,TInput input)
        {
            var processor = DependencyResolver.Current.GetService<IVMProcess<TContext,TView, TInput>>();

            if (processor != null)
                return processor.Process(_Context, view,input );
            RevTrace.TE("couldnt resolve");

            throw new NotImplementedException();
        }

        private TContext _Context;
    }


    /// <summary>
    /// Interface to populate a model, concrete implementation is in the derived class, no param
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public interface IVMProcess<TContext, TView>
    {
        IResponse Process(TContext ctx, TView view);
    }

    /// <summary>
    /// Interface to populate a model, concrete implementation is in the derived class, with param
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public interface IVMProcess<TContext,TView, TInput>
    {
        IResponse Process(TContext ctx,TView view, TInput input);
    }

    
}