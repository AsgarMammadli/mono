//
// System.Nullable
//
// Martin Baulig (martin@ximian.com)
// Marek Safar	 (marek.safar@gmail.com)
//
// (C) 2004 Novell, Inc.
//

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if NET_2_0

using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible (true)]
	public static class Nullable {
		public static int Compare<T> (T? value1, T? value2) where T: struct
		{
			if (value1.has_value) {
				if (!value2.has_value)
					return 1;

				return Comparer<T>.Default.Compare (value1.value, value2.value);
			}
			
			return value2.has_value ? -1 : 0;
		}

		public static bool Equals<T> (T? value1, T? value2) where T: struct
		{
			if (value1.has_value != value2.has_value)
				return false;

			if (!value1.has_value)
				return true;

			return EqualityComparer<T>.Default.Equals (value1.value, value2.value);
		}

		public static Type GetUnderlyingType (Type nullableType)
		{
			if (nullableType == null)
				throw new ArgumentNullException ("nullableType");
			if (nullableType.IsGenericType && nullableType.GetGenericTypeDefinition () == typeof (Nullable<>))
				return nullableType.GetGenericArguments ()[0];
			else
				return null;
		}
	}

	[Serializable]
	public struct Nullable<T> where T: struct
	{
		#region Sync with runtime code
		internal T value;
		internal bool has_value;
		#endregion

		public Nullable (T value)
		{
			this.has_value = true;
			this.value = value;
		}

		public bool HasValue {
			get { return has_value; }
		}

		public T Value {
			get { 
				if (!has_value)
					throw new InvalidOperationException ("Nullable object must have a value.");
				
				return value; 
			}
		}

		public override bool Equals (object other)
		{
			if (other == null)
				return has_value == false;
			if (!(other is Nullable<T>))
				return false;

			return Equals ((Nullable <T>) other);
		}

		bool Equals (Nullable<T> other)
		{
			if (other.has_value != has_value)
				return false;

			if (has_value == false)
				return true;

			return other.value.Equals (value);
		}

		public override int GetHashCode ()
		{
			if (!has_value)
				return 0;

			return value.GetHashCode ();
		}

		public T GetValueOrDefault ()
		{
			return GetValueOrDefault (default (T));
		}

		public T GetValueOrDefault (T def_value)
		{
			if (!has_value)
				return def_value;
			else
				return value;
		}

		public override string ToString ()
		{
			if (has_value)
				return value.ToString ();
			else
				return String.Empty;
		}

		public static implicit operator Nullable<T> (T value)
		{
			return new Nullable<T> (value);
		}

		public static explicit operator T (Nullable<T> value)
		{
			return value.Value;
		}

		// These are called by the JIT
		// Ironicly, the C#  code is the same for these two,
		// however, on the inside they do somewhat different things
		static object Box (T? o)
		{
			if (o == null)
				return null;
			return (T) o;
		}
		
		static T? Unbox (object o)
		{
			if (o == null)
				return null;
			return (T) o;
		}
	}
}
#endif
