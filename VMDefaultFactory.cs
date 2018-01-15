using System;
using System.Web.Mvc;
using DBoard.DAL;
using DBoard.Models.Views;
using Revo.Utilities;

namespace DBoard.Utilities.VMFactory
{
    /// <summary>
    /// Interface of the factory to build views
    /// Dont pass a controller as TInput itself to the view factory to keep it loose coupled
    /// got the basic idea here: http://benfoster.io/blog/using-the-view-factory-pattern-in-aspnet-mvc
    /// </summary>
    public interface IVMFactory
    {
        /// <summary>
        /// Creates a view without parameter
        /// <see cref="DBoard.Utilities.VMFactory.VMFactoryBindings.RegisterBindings(Ninject.IKernel)"/>
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <returns></returns>
        TView CreateView<TView>();

        /// <summary>
        /// Creates view with paramer of type TInput
        /// <see cref="DBoard.Utilities.VMFactory.VMFactoryBindings.RegisterBindings(Ninject.IKernel)"/>
        /// </summary>
        /// <typeparam name="TView">type of view to build</typeparam>
        /// <typeparam name="TInput">parameter type that is passed to the factory</typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        TView CreateView<TView, TInput>(TInput input);

        /// <summary>
        /// Creates view with paramer of type TInput i1 and TInput2 i2
        /// <see cref="DBoard.Utilities.VMFactory.VMFactoryBindings.RegisterBindings(Ninject.IKernel)"/>
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TInput2"></typeparam>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        TView CreateView<TView, TInput, TInput2>(TInput i1,TInput2 i2);
    }

    public class VMDefaultFactory<TContext> : IVMFactory
    {
        /// <summary>
        /// Ctor of factory, Ninject injection DI can also be used
        /// IoTContext is the db context used when built
        /// </summary>
        /// <param name="context"></param>
        public VMDefaultFactory(TContext context)
        {
            _Context = context;
        }

        /// <summary>
        /// Creates a view of type TView without parameter. Usage:
        /// <para>1. Define VMExampleData  -> model class, passed to view</para>
        /// <para>2. Define VMExampleDataBuilder -> how to populate model class</para>
        /// <para>3. Bind IVMFactory to concrete  VMExampleDataBuilder VMExampleData in VMFactoryBindings</para>
        /// <para>VMFactoryBindings.RegisterBindings: register bindings there. VMBuilders.cs File for simple builders</para>
        /// <see cref="DBoard.Utilities.VMFactory.VMFactoryBindings.RegisterBindings(Ninject.IKernel)"/>
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <returns></returns>
        public TView CreateView<TView>()
        {
            var builder = DependencyResolver.Current.GetService<IVMBuilder<TContext,TView>>();

            if (builder != null)
                return builder.Build(_Context);
            RevTrace.TE("couldnt resolve");
            // otherwise create view using reflection
            return Activator.CreateInstance<TView>();
        }

        /// <summary>
        /// Creates a view of type TView with parameter of type TInput. Usage:
        /// <para>1. Define VMExampleData  -> model class, passed to view</para>
        /// <para>2. Define VMExampleDataBuilder -> how to populate model class</para>
        /// <para>3. Bind IVMFactory to concrete  VMExampleDataBuilder VMExampleData in VMFactoryBindings</para>
        /// <para>VMFactoryBindings.RegisterBindings -> register bindings there. VMBuilders.cs -> File for simple builders</para>
        /// <see cref="DBoard.Utilities.VMFactory.VMFactoryBindings.RegisterBindings(Ninject.IKernel)"/>
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <returns></returns>
        public TView CreateView<TView, TInput>(TInput input)
        {
            var builder = DependencyResolver.Current.GetService<IVMBuilder<TContext,TView,TInput>>();

            if (builder != null)
                return builder.Build(_Context, input);
            RevTrace.TE("couldnt resolve");
            // otherwise create using reflection
            return (TView)Activator.CreateInstance(typeof(TView), input);
        }

        /// <summary>
        /// Creates a view of type TView with parameter of type TInput and TInput2. Usage:
        /// <para>1. Define VMExampleData  -> model class, passed to view</para>
        /// <para>2. Define VMExampleDataBuilder -> how to populate model class</para>
        /// <para>3. Bind IVMFactory to concrete  VMExampleDataBuilder VMExampleData in VMFactoryBindings</para>
        /// <para>VMFactoryBindings.RegisterBindings -> register bindings there. VMBuilders.cs -> File for simple builders</para>
        /// <see cref="DBoard.Utilities.VMFactory.VMFactoryBindings.RegisterBindings(Ninject.IKernel)"/>
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <returns></returns>
        public TView CreateView<TView, TInput, TInput2>(TInput i1, TInput2 i2)
        {
            var builder = DependencyResolver.Current.GetService<IVMBuilder<TContext, TView, TInput, TInput2>>();

            if (builder != null)
                return builder.Build(_Context, i1, i2);
            RevTrace.TE("couldnt resolve");
            // otherwise create using reflection
            return (TView)Activator.CreateInstance(typeof(TView), i1, i2);
        }

        private TContext _Context;
    }

    public static class VMDefaultFactoryExtension
    {
        /// <summary>
        /// Add VMShared to model if it is inherited by IHasVMShared
        /// </summary>
        /// <param name="fac"></param>
        /// <param name="shared"></param>
        public static void AddShared(this VMDefaultFactory<AppDBContext> fac, IHasVMShared shared)
        {
            shared.Shared=fac.CreateView<VMShared>();
        }
    }

    /// <summary>
    /// Interface to populate a model, concrete implementation is in the derived class, no param
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public interface IVMBuilder<TContext,TView>
    {
        TView Build(TContext ctx);
    }

    /// <summary>
    /// Interface to populate a model, concrete implementation is in the derived class, with param
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public interface IVMBuilder<TContext, TView, TInput>
    {
        TView Build(TContext ctx, TInput input);
    }

    /// <summary>
    /// Interface to populate a model, concrete implementation is in the derived class, with param
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    public interface IVMBuilder<TContext, TView, TInput, TInput2>
    {
        TView Build(TContext ctx, TInput i1, TInput2 i2);
    }

    /// <summary>
    /// Interface is used to build a has a relationship with any type which can be used in the factory
    /// </summary>
    public interface IHasVMShared
    {
        VMShared Shared { get; set; }
    }

   
   

    
}