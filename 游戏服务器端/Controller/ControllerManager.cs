using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Reflection;

namespace GameServer.Controller
{
    class ControllerManager
    {
        private Dictionary<RequestCode, BaseController> controllerDict = new Dictionary<RequestCode, BaseController>();

        public ControllerManager()
        {
            Init();
        }
        private void Init()
        {
            DefaultController defaultController = new DefaultController();
            controllerDict.Add(defaultController.RequestCode, defaultController);
        }
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="actionCode"></param>
        public void HandleRequest(RequestCode requestCode,ActionCode actionCode,string data)
        {
            BaseController controller;
            bool isGet = controllerDict.TryGetValue(requestCode, out controller);
            if (isGet == false)
            {
                Console.WriteLine("无法得到"+requestCode+"多对应的controller，无法处理请求");return;
            }
            string methodName = Enum.GetName(typeof(ActionCode),actionCode);
            MethodInfo mi = controller.GetType().GetMethod(methodName);
            if (mi == null)
            {
                Console.WriteLine("【警告】在controller["+controller.GetType()+"]中没有对应的处理方法：【"+methodName+"】");return;
            }
            object[] parameters = new object[] { data };
            mi.Invoke(controller, parameters);
        }
    }
}
