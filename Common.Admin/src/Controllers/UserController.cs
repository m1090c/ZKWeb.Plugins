﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZKWeb.Plugins.Common.Admin.src.Extensions;
using ZKWeb.Plugins.Common.Admin.src.Forms;
using ZKWeb.Plugins.Common.Admin.src.Managers;
using ZKWeb.Plugins.Common.Admin.src.Model;
using ZKWeb.Plugins.Common.Base.src;
using ZKWeb.Plugins.Common.Base.src.Managers;
using ZKWeb.Utils.Functions;
using ZKWeb.Utils.IocContainer;
using ZKWeb.Web.ActionResults;
using ZKWeb.Web.Interfaces;

namespace ZKWeb.Plugins.Common.Admin.src.Controllers {
	/// <summary>
	/// 用户控制器
	/// </summary>
	[ExportMany]
	public class UserController : IController {
		/// <summary>
		/// 注册用户
		/// </summary>
		/// <returns></returns>
		[Action("user/reg")]
		[Action("user/reg", HttpMethods.POST)]
		public IActionResult Reg() {
			// 已登录时跳转到用户中心
			var sessionManager = Application.Ioc.Resolve<SessionManager>();
			var user = sessionManager.GetSession().GetUser();
			if (user != null) {
				return new RedirectResult("/home");
			}
			// 否则显示注册表单
			var form = new UserRegForm();
			if (HttpContextUtils.CurrentContext.Request.HttpMethod == HttpMethods.POST) {
				return new JsonResult(form.Submit());
			} else {
				form.Bind();
				return new TemplateResult("common.admin/user_reg.html", new { form });
			}
		}

		/// <summary>
		/// 登陆用户
		/// </summary>
		/// <returns></returns>
		[Action("user/login")]
		[Action("user/login", HttpMethods.POST)]
		public IActionResult Login() {
			// 已登录时跳转到用户中心
			var sessionManager = Application.Ioc.Resolve<SessionManager>();
			var user = sessionManager.GetSession().GetUser();
			if (user != null) {
				return new RedirectResult("/home");
			}
			// 否则显示登陆表单
			var form = new UserLoginForm();
			if (HttpContextUtils.CurrentContext.Request.HttpMethod == HttpMethods.POST) {
				return new JsonResult(form.Submit());
			} else {
				form.Bind();
				return new TemplateResult("common.admin/user_login.html", new { form });
			}
		}

		/// <summary>
		/// 退出登录
		/// </summary>
		/// <returns></returns>
		[Action("user/logout", HttpMethods.POST)]
		public IActionResult Logout() {
			var userManager = Application.Ioc.Resolve<UserManager>();
			userManager.Logout();
			return new RedirectResult("/user/login");
		}

		/// <summary>
		/// 用户中心
		/// </summary>
		/// <returns></returns>
		[Action("home")]
		public IActionResult Home() {
			var privilegeManager = Application.Ioc.Resolve<PrivilegeManager>();
			privilegeManager.Check(UserTypesGroup.All);
			return new TemplateResult("common.admin/user_home.html");
		}
	}
}
