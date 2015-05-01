using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Security.Controllers;
using Xpand.ExpressApp.Security.Registration;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Security;
using Xpand.Persistent.Base.Validation;
using ChooseDatabaseAtLogonController = Xpand.ExpressApp.Security.Controllers.ChooseDatabaseAtLogonController;

namespace Xpand.ExpressApp.Security {
    public abstract class XpandSecurityModuleBase:XpandModuleBase {
        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.SetupComplete+=ApplicationOnSetupComplete;
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            ((XafApplication) sender).CreateCustomLogonWindowControllers += application_CreateCustomLogonWindowControllers;
        }

        private void application_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            if (((IModelOptionsRegistration) Application.Model.Options).Registration.Enabled)
                AddRegistrationControllers(sender, e);
            if (((IModelOptionsChooseDatabaseAtLogon) Application.Model.Options).ChooseDatabaseAtLogon){
                e.Controllers.Add(Application.CreateController<ChooseDatabaseAtLogonController>());
                e.Controllers.AddRange(Application.CreateAppearenceControllers());
                e.Controllers.AddRange(Application.CreateValidationControllers());
            }
        }

        protected virtual void AddRegistrationControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            var app = (XafApplication) sender;
            e.Controllers.AddRange(CreateRegistrationControllers(app));
            e.Controllers.AddRange(app.CreateAppearenceControllers());
            e.Controllers.AddRange(app.CreateValidationControllers());
        }

        public static IEnumerable<Controller> CreateRegistrationControllers(XafApplication app) {
            var typeInfo = app.TypesInfo.FindTypeInfo(typeof(IPasswordScoreController)).Implementors.FirstOrDefault();
            if (typeInfo != null)
                yield return app.CreateController(typeInfo.Type);

            yield return app.CreateController<ManageUsersOnLogonController>();
        }
    }
}
