/*
Copyright 2010 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Google.Apis.JSON;
using Google.Apis.Requests;
namespace Google.Apis.Discovery
{
	public class Method
	{
		private JSONDictionary information;
		public string Name {get;set;}
		private Dictionary<string, Parameter> parameters;

		private Method ()
		{
		}

		internal Method (KeyValuePair<string, object> kvp)
		{
			this.Name = kvp.Key;
			this.information = kvp.Value as JSONDictionary;
			if (this.information == null)
				throw new ArgumentException ("got no valid dictionary");
		}

		public string RestPath {
			get { return this.information[ServiceFactory.discovery_pathUrl] as string; }
		}

		public string RpcName {
			get { return this.information[ServiceFactory.discovery_rpcName] as string; }
		}

		public string HttpMethod {
			get { return this.information.GetValueAsNull (ServiceFactory.discovery_httpMethod) as string; }
		}

		public Dictionary<string, Parameter> Parameters {
			get {
				if (this.parameters == null) {
					JSONDictionary js = this.information[ServiceFactory.discovery_parameters] as JSONDictionary;
					if (js != null) {
						this.parameters = new Dictionary<string, Parameter> ();
						foreach (KeyValuePair<string, object> kvp in js) {
							Parameter p = new Parameter (kvp);
							this.parameters.Add (kvp.Key, p);
						}
					}
				}
				return this.parameters;
			}
		}
	}
}