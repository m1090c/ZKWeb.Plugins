﻿using System;
using ZKWeb.Localize;
using ZKWeb.Plugins.Common.Admin.src.Domain.Extensions;
using ZKWeb.Plugins.Common.Base.src.Domain.Services;
using ZKWeb.Plugins.Common.Base.src.UIComponents.Forms;
using ZKWeb.Plugins.Common.Base.src.UIComponents.Forms.Attributes;
using ZKWeb.Plugins.Common.Base.src.UIComponents.ScriptStrings;
using ZKWeb.Plugins.Shopping.Order.src.Domain.Services;
using ZKWebStandard.Extensions;

namespace ZKWeb.Plugins.Shopping.Order.src.UIComponents.Forms {
	/// <summary>
	/// 后台代替买家确认收货的表单
	/// </summary>
	[Form("OrderConfirmInsteadOfBuyerForm", SubmitButtonText = "ConfirmOrder")]
	public class OrderConfirmInsteadOfBuyerForm : ModelFormBuilder {
		/// <summary>
		/// 绑定表单
		/// </summary>
		protected override void OnBind() { }

		/// <summary>
		/// 提交表单
		/// </summary>
		protected override object OnSubmit() {
			var orderId = Request.Get<Guid>("id");
			var sessionManager = Application.Ioc.Resolve<SessionManager>();
			var user = sessionManager.GetSession().GetUser();
			var orderManager = Application.Ioc.Resolve<SellerOrderManager>();
			var message = orderManager.ConfirmOrder(orderId, user?.Id, false) ?
				new T("Confirm order success") :
				new T("Confirm order failed, please check order records");
			return new { message = message, script = BaseScriptStrings.AjaxtableUpdatedAndCloseModal };
		}
	}
}
