﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZKWeb.Web.ActionResults;
using ZKWeb.Plugins.Common.Admin.src;
using ZKWeb.Plugins.Common.Admin.src.Extensions;
using ZKWeb.Plugins.Common.Admin.src.Managers;
using ZKWeb.Plugins.Common.Admin.src.Model;
using ZKWeb.Plugins.Common.Base.src;
using ZKWeb.Plugins.Common.Base.src.Extensions;
using ZKWeb.Plugins.Common.Base.src.Model;
using ZKWeb.Web.Interfaces;
using ZKWeb.Utils.Functions;

namespace ZKWeb.Plugins.Common.MenuPageBase.src.Scaffolding {
	/// <summary>
	/// 带单个表单的菜单页面构建器
	/// 这个抽象类需要再次继承，请勿直接使用
	/// <example>
	/// public abstract class GenericFormForUserPanel :
	///		GenericFormForMenuPage, IMenuProviderForUserPanel { }
	/// [ExportMany]
	/// public class ExampleForm : GenericFormForUserPanel { }
	/// </example>
	/// </summary>
	public abstract class GenericFormForMenuPage : GenericPageForMenuPage {
		/// <summary>
		/// 获取表单
		/// </summary>
		/// <returns></returns>
		protected abstract IModelFormBuilder GetForm();

		/// <summary>
		/// 请求的处理函数
		/// </summary>
		protected override IActionResult Action() {
			// 检查权限
			var privilegeManager = Application.Ioc.Resolve<PrivilegeManager>();
			privilegeManager.Check(AllowedUserTypes, RequiredPrivileges);
			// 处理绑定和提交
			var form = GetForm();
			if (HttpContextUtils.CurrentContext.Request.HttpMethod == HttpMethods.POST) {
				return new JsonResult(form.Submit());
			} else {
				form.Bind();
				return new TemplateResult(TemplatePath, new { title = Name, iconClass = IconClass, form });
			}
		}
	}
}
