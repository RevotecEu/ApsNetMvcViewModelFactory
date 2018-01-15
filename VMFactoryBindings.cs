using DBoard.Core.Security;
using DBoard.DAL;
using DBoard.Models;
using DBoard.Models.Builder;
using DBoard.Models.Process;
using DBoard.Models.Views;
using Ninject;

namespace DBoard.Utilities.VMFactory
{
    public static class VMFactoryBindings
    {
        /// <summary>
        /// Define binding between VMModel and corresponding builder class here
        /// Gets automatically called by Ninjet
        /// <see cref="DBoard.App_Start.NinjectWebCommon.RegisterServices(IKernel)"/>
        /// </summary>
        /// <param name="kernel"></param>
        public static void RegisterBindings(IKernel kernel)
        {
            // builders 
            kernel.Bind(typeof(IVMBuilder<AppDBContext, VMShared>)).To(typeof(VMSharedBuilder));  //without param

        }
    }
}