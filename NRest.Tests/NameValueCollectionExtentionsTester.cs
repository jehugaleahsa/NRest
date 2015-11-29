using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NRest.Tests
{
    [TestClass]
    public class NameValueCollectionExtentionsTester
    {
        [TestMethod]
        public void ShouldSetStringProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["StringValue"] = "Test";

            TestClass instance = collection.Create<TestClass>();

            Assert.AreEqual("Test", instance.StringValue, "The String property was not set.");
        }

        [TestMethod]
        public void ShouldSetByteProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["ByteValue"] = "123";

            TestClass instance = collection.Create<TestClass>();

            Assert.AreEqual(123, instance.ByteValue, "The Byte property was not set.");
        }

        [TestMethod]
        public void ShouldSetInt16Property()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["Int16Value"] = "123";

            TestClass instance = collection.Create<TestClass>();

            Assert.AreEqual(123, instance.Int16Value, "The Int16 property was not set.");
        }

        [TestMethod]
        public void ShouldSetInt32Property()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["Int32Value"] = "123";

            TestClass instance = collection.Create<TestClass>();

            Assert.AreEqual(123, instance.Int32Value, "The Int32 property was not set.");
        }

        [TestMethod]
        public void ShouldSetInt64Property()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["Int64Value"] = "123";

            TestClass instance = collection.Create<TestClass>();

            Assert.AreEqual(123, instance.Int64Value, "The Int64 property was not set.");
        }

        [TestMethod]
        public void ShouldSetSingleProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["SingleValue"] = "12.34";

            TestClass instance = collection.Create<TestClass>();

            Assert.AreEqual(12.34f, instance.SingleValue, "The Single property was not set.");
        }

        [TestMethod]
        public void ShouldSetDoubleProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["DoubleValue"] = "12.34";

            TestClass instance = collection.Create<TestClass>();

            Assert.AreEqual(12.34, instance.DoubleValue, "The Double property was not set.");
        }

        [TestMethod]
        public void ShouldSetDecimalProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["DecimalValue"] = "12.34";

            TestClass instance = collection.Create<TestClass>();

            Assert.AreEqual(12.34m, instance.DecimalValue, "The Decimal property was not set.");
        }

        [TestMethod]
        public void ShouldSetNullableByteProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["NullableByteValue"] = "123";

            TestClass instance = collection.Create<TestClass>();

            Assert.AreEqual((byte?)123, instance.NullableByteValue, "The Byte property was not set.");
        }

        [TestMethod]
        public void ShouldSetNullableInt16Property()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["NullableInt16Value"] = "123";

            TestClass instance = collection.Create<TestClass>();

            Assert.AreEqual((short?)123, instance.NullableInt16Value, "The Int16 property was not set.");
        }

        [TestMethod]
        public void ShouldSetNullableInt32Property()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["NullableInt32Value"] = "123";

            TestClass instance = collection.Create<TestClass>();

            Assert.AreEqual(123, instance.NullableInt32Value, "The Int32 property was not set.");
        }

        [TestMethod]
        public void ShouldSetNullableInt64Property()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["NullableInt64Value"] = "123";

            TestClass instance = collection.Create<TestClass>();

            Assert.AreEqual(123, instance.NullableInt64Value, "The Int64 property was not set.");
        }

        [TestMethod]
        public void ShouldSetNullableSingleProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["NullableSingleValue"] = "12.34";

            TestClass instance = collection.Create<TestClass>();

            Assert.AreEqual(12.34f, instance.NullableSingleValue, "The Single property was not set.");
        }

        [TestMethod]
        public void ShouldSetNullableDoubleProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["NullableDoubleValue"] = "12.34";

            TestClass instance = collection.Create<TestClass>();

            Assert.AreEqual(12.34, instance.NullableDoubleValue, "The Double property was not set.");
        }

        [TestMethod]
        public void ShouldSetNullableDecimalProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection["NullableDecimalValue"] = "12.34";

            TestClass instance = collection.Create<TestClass>();

            Assert.AreEqual(12.34m, instance.NullableDecimalValue, "The Decimal property was not set.");
        }

        [TestMethod]
        public void ShouldSetNonGenericEnumerableProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("NonGenericEnumerable", "Hello");
            collection.Add("NonGenericEnumerable", "World");

            TestClass instance = collection.Create<TestClass>();
            List<string> expected = new List<string>() { "Hello", "World" };

            Assert.IsInstanceOfType(instance.NonGenericEnumerable, typeof(List<string>), "The collection should be a List<string>.");
            CollectionAssert.AreEqual(expected, (List<string>)instance.NonGenericEnumerable, "The values were not set in an IEnumerable.");
        }

        [TestMethod]
        public void ShouldIgnoreNonGenericArrayProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("NonGenericArray", "Hello");
            collection.Add("NonGenericArray", "World");

            TestClass instance = collection.Create<TestClass>();

            Assert.IsNull(instance.NonGenericArray, "The Array should have been ignored.");
        }

        [TestMethod]
        public void ShouldSetStringArrayProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("StringArray", "Hello");
            collection.Add("StringArray", "World");

            TestClass instance = collection.Create<TestClass>();
            string[] expected = new string[] { "Hello", "World" };

            CollectionAssert.AreEqual(expected, instance.StringArray, "The values were not set in an string[].");
        }

        [TestMethod]
        public void ShouldSetStringICollectionProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("StringICollection", "Hello");
            collection.Add("StringICollection", "World");

            TestClass instance = collection.Create<TestClass>();
            string[] expected = new string[] { "Hello", "World" };

            Assert.IsInstanceOfType(instance.StringICollection, typeof(List<string>), "The collection should be a List<string>.");
            CollectionAssert.AreEqual(expected, (List<string>)instance.StringICollection, "The values were not set in an ICollection.");
        }

        [TestMethod]
        public void ShouldSetStringIListProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("StringIList", "Hello");
            collection.Add("StringIList", "World");

            TestClass instance = collection.Create<TestClass>();
            string[] expected = new string[] { "Hello", "World" };

            Assert.IsInstanceOfType(instance.StringIList, typeof(List<string>), "The collection should be a List<string>.");
            CollectionAssert.AreEqual(expected, (List<string>)instance.StringIList, "The values were not set in an IList.");
        }

        [TestMethod]
        public void ShouldSetStringISetProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("StringISet", "Hello");
            collection.Add("StringISet", "World");

            TestClass instance = collection.Create<TestClass>();
            HashSet<string> expected = new HashSet<string>() { "Hello", "World" };

            Assert.IsInstanceOfType(instance.StringISet, typeof(HashSet<string>), "The collection should be a HashSet<string>.");
            Assert.IsTrue(expected.SetEquals(instance.StringISet), "The values were not set in an ISet.");
        }

        [TestMethod]
        public void ShouldSetStringListProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("StringList", "Hello");
            collection.Add("StringList", "World");

            TestClass instance = collection.Create<TestClass>();
            List<string> expected = new List<string>() { "Hello", "World" };

            CollectionAssert.AreEqual(expected, instance.StringList, "The values were not set in a List<string>.");
        }

        [TestMethod]
        public void ShouldSetStringSetProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("StringSet", "Hello");
            collection.Add("StringSet", "World");

            TestClass instance = collection.Create<TestClass>();
            HashSet<string> expected = new HashSet<string>() { "Hello", "World" };

            Assert.IsTrue(expected.SetEquals(instance.StringSet), "The values were not set in a HashSet.");
        }

        [TestMethod]
        public void ShouldSetNullableInt32ArrayProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("NullableInt32Array", "123");
            collection.Add("NullableInt32Array", "345");

            TestClass instance = collection.Create<TestClass>();
            int?[] expected = new int?[] { 123, 345 };

            CollectionAssert.AreEqual(expected, instance.NullableInt32Array, "The values were not set in an int?[].");
        }

        [TestMethod]
        public void ShouldSetNullableInt32ICollectionProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("NullableInt32ICollection", "123");
            collection.Add("NullableInt32ICollection", "345");

            TestClass instance = collection.Create<TestClass>();
            int?[] expected = new int?[] { 123, 345 };

            Assert.IsInstanceOfType(instance.NullableInt32ICollection, typeof(List<int?>), "The collection should be a List<int?>.");
            CollectionAssert.AreEqual(expected, (List<int?>)instance.NullableInt32ICollection, "The values were not set in an ICollection.");
        }

        [TestMethod]
        public void ShouldSetNullableInt32IListProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("NullableInt32IList", "123");
            collection.Add("NullableInt32IList", "345");

            TestClass instance = collection.Create<TestClass>();
            int?[] expected = new int?[] { 123, 345 };

            Assert.IsInstanceOfType(instance.NullableInt32IList, typeof(List<int?>), "The collection should be a List<int?>.");
            CollectionAssert.AreEqual(expected, (List<int?>)instance.NullableInt32IList, "The values were not set in an IList.");
        }

        [TestMethod]
        public void ShouldSetNullableInt32ISetProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("NullableInt32ISet", "123");
            collection.Add("NullableInt32ISet", "345");

            TestClass instance = collection.Create<TestClass>();
            HashSet<int?> expected = new HashSet<int?>() { 123, 345 };

            Assert.IsInstanceOfType(instance.NullableInt32ISet, typeof(HashSet<int?>), "The collection should be a HashSet<int?>.");
            Assert.IsTrue(expected.SetEquals(instance.NullableInt32ISet), "The values were not set in an ISet.");
        }

        [TestMethod]
        public void ShouldSetNullableInt32ListProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("NullableInt32List", "123");
            collection.Add("NullableInt32List", "345");

            TestClass instance = collection.Create<TestClass>();
            List<int?> expected = new List<int?>() { 123, 345 };

            CollectionAssert.AreEqual(expected, instance.NullableInt32List, "The values were not set in a List<int?>.");
        }

        [TestMethod]
        public void ShouldSetNullableInt32SetProperty()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("NullableInt32Set", "123");
            collection.Add("NullableInt32Set", "345");

            TestClass instance = collection.Create<TestClass>();
            HashSet<int?> expected = new HashSet<int?>() { 123, 345 };

            Assert.IsTrue(expected.SetEquals(instance.NullableInt32Set), "The values were not set in a HashSet.");
        }

        public class TestClass
        {
            public string StringValue { get; set; }

            public byte ByteValue { get; set; }

            public short Int16Value { get; set; }

            public int Int32Value { get; set; }

            public long Int64Value { get; set; }

            public float SingleValue { get; set; }

            public double DoubleValue { get; set; }

            public decimal DecimalValue { get; set; }

            public byte? NullableByteValue { get; set; }

            public short? NullableInt16Value { get; set; }

            public int? NullableInt32Value { get; set; }

            public long? NullableInt64Value { get; set; }

            public float? NullableSingleValue { get; set; }

            public double? NullableDoubleValue { get; set; }

            public decimal? NullableDecimalValue { get; set; }

            public IEnumerable NonGenericEnumerable { get; set; }

            public Array NonGenericArray { get; set; }

            public string[] StringArray { get; set; }

            public ICollection<string> StringICollection { get; set; }

            public IList<string> StringIList { get; set; }

            public ISet<string> StringISet { get; set; }

            public List<string> StringList { get; set; }

            public HashSet<string> StringSet { get; set; }

            public int?[] NullableInt32Array { get; set; }

            public ICollection<int?> NullableInt32ICollection { get; set; }

            public IList<int?> NullableInt32IList { get; set; }

            public ISet<int?> NullableInt32ISet { get; set; }

            public List<int?> NullableInt32List { get; set; }

            public HashSet<int?> NullableInt32Set { get; set; }
        }

        [TestMethod]
        public void ShouldBuildUserEntity()
        {
            NameValueCollection collection = new NameValueCollection();
            collection.Add("testuserid", "123");
            collection.Add("username", "tparks");
            collection.Add("birthday", "7-04-1933");
            collection.Add("totalposts", null);
            collection.Add("posts", "This is my first post");
            collection.Add("posts", "I really like toast");

            TestUser instance = collection.Create<TestUser>();

            Assert.AreEqual(123, instance.TestUserId, "The user ID was wrong.");
            Assert.AreEqual("tparks", instance.UserName, "The user name was wrong.");
            Assert.AreEqual(new DateTime(1933, 07, 04), instance.Birthday, "The birthday was wrong.");
            Assert.IsNull(instance.TotalPosts, "The total post should be null.");
            var expectedPosts = new string[] { "This is my first post", "I really like toast" };
            CollectionAssert.AreEquivalent(expectedPosts, instance.Posts, "The posts were not set.");
        }

        public class TestUser
        {
            public int TestUserId { get; set; }

            public string UserName { get; set; }

            public DateTime? Birthday { get; set; }

            public int? TotalPosts { get; set; }

            public string[] Posts { get; set; }
        }
    }
}
