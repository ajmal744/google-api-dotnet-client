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
	public static class Utilities
	{
		public static TValue GetValueAsNull<TKey, TValue> (this IDictionary<TKey, TValue> data, TKey key)
		{
			TValue result;
			if (!data.TryGetValue (key, out result)) {
				return default(TValue);
			}
			return result;
		}
	}
}