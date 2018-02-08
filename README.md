**General View Factory Pattern for C#**


There are a few good reasons for using the View Factory Pattern in C#
- It keeps the logic in your controller actions to a miniumum.
- It avoids constructor soup.
- Your views (or view models) can be tested without any dependency on MVC framework code.

**Usage:**
PM add Ninjet, we use it as dependency resolver


1. IKernel kernel get your ninjet kernel
2. kernel.Bind(typeof(IVMBuilder<IoTContext, MyVM>)).To(typeof(MyVMBuilder));
3. Build A Builder based on the template for creating the ViewModel

public class MyVMBuilder: IVMBuilder<DBContext,MyVm>
    {
        public MyVM Build(IoTContext context)
        {
          // do what needs to be done
        }
    };
