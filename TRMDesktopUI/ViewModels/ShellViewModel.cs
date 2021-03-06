﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
	public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
	{
		private IEventAggregator _events;
		private SalesViewModel _salesVM;
		private ILoggedInUserModel _user;

		public ShellViewModel(IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel user)
		{
			_events = events;
			_salesVM = salesVM;
			_user = user;

			_events.Subscribe(this);
			
			ActivateItem(IoC.Get<LoginViewModel>());
		}

		public bool IsALoggedIn
		{
			get
			{
				bool output = false;
				if (string.IsNullOrWhiteSpace(_user.Token) == false)
				{
					output = true;
				}
				return output;
			}
		}

		public void ExitApplication()
		{
			TryClose();
		}

		public void LogOut()
		{
			_user.ResetUserModel();
			ActivateItem(IoC.Get<LoginViewModel>());
			NotifyOfPropertyChange(() => IsALoggedIn);

		}

		public void Handle(LogOnEvent message)
		{
			ActivateItem(_salesVM);
			NotifyOfPropertyChange(() => IsALoggedIn);
		}


	}
}
