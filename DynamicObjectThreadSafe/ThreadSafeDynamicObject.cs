using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DynamicObjectThreadSafe
{
	public class ThreadSafeDynamicObject : DynamicObject, IEnumerable<KeyValuePair<string, object>>
	{

		public ThreadSafeDynamicObject()
		{
		}

		public ThreadSafeDynamicObject(dynamic members)
		{
			dynamic membersDict = ToDictionary(members);
			InitMembers(membersDict);
		}

		private IDictionary<string, object> ToDictionary(object data)
		{
			var attr = BindingFlags.Public | BindingFlags.Instance;
			var dict = new Dictionary<string, object>();
			foreach (var property in data.GetType().GetProperties(attr))
			{
				if (property.CanRead)
				{
					dict.Add(property.Name, property.GetValue(data, null));
				}
			}
			return dict;
		}

		private void InitMembers(IDictionary<string, object> membersDict)
		{
			foreach (KeyValuePair<string, object> member in membersDict)
			{
				_members.AddOrUpdate(member.Key, member.Value, (key, oldValue) => member.Value);
			}
		}

		private readonly ConcurrentDictionary<string, object> _members = new ConcurrentDictionary<string, object>();

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return _members.TryGetValue(binder.Name, out result);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			_members.AddOrUpdate(binder.Name, value, (key, oldvalue) => value);
			return true;
		}

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return _members.Keys.ToList().AsReadOnly();
		}

		public override string ToString()
		{
			return JsonSerializer.Serialize(_members);
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return _members.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _members.GetEnumerator();
		}
	}

}
