﻿// Copyright 2007-2011 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Topshelf.Hosts
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ServiceProcess;
	using Configuration;
	using log4net;
	using Windows;
	using WindowsServiceCode;


	public class UninstallHost :
		AbstractInstallerHost,
		Host
	{
		readonly ILog _log = LogManager.GetLogger("Topshelf.Hosts.UninstallHost");


		public UninstallHost(ServiceDescription description, ServiceStartMode startMode, IEnumerable<string> dependencies,
		                     Credentials credentials, IEnumerable<Action> preActions, IEnumerable<Action> postActions)
			: base(description, startMode, dependencies, credentials, preActions, postActions)
		{
		}

		public void Run()
		{
			if (!WindowsServiceControlManager.IsInstalled(Description.GetServiceName()))
			{
				_log.ErrorFormat("The {0} service is not installed.", Description.GetServiceName());
				return;
			}

			if (!WindowsUserAccessControl.IsAdministrator)
			{
				if (!WindowsUserAccessControl.RerunAsAdministrator())
					_log.ErrorFormat("The {0} service can only be uninstalled as an administrator", Description.GetServiceName());

				return;
			}

			_log.DebugFormat("Attempting to uninstall '{0}'", Description.GetServiceName());

			WithInstaller(ti => ti.Uninstall(null));
		}
	}
}