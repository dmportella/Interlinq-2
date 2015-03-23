//Copyright © DMPortella.  All Rights Reserved.
//This code released under the terms of the 
//Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

//Copyright © DMPortella.  All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using InterLinq;
using InterLinq.Expressions;
using InterLinq.Types;
using InterLinq.Tests.Entities;
using InterLinq.Tests.Client;
using InterLinq.Tests.Server.Model;

namespace InterLinq.Tests
{
    [TestClass]
    public class SerializationTests
    {
        private Client.IMockClientContext clientContext;

        private MockObjectRepository repository;

        [TestInitialize]
        public void Initialize()
        {
            clientContext = new Client.MockClientContext();
            repository = new MockObjectRepository();
        }

        [TestCategory("Restriction Operators")]
        [TestMethod]//[Title("Where_Simple 1")]
        [Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
        public void Where_Simple_1()
        {
            var cityCustomers = (from c in this.clientContext.Customers.CustomersByCity("London")
                                where c.Orders.Count() > 3
                                select c).ToList();

            var cityCustomers2 = (from c in this.clientContext.CustomersByCity("Berlin")
                                 where c.Orders.Count() > 3
                                 select c).ToList();

            var expected = (from num in this.repository.GetProductList()
                           where num.ProductID < 5
                           select num).ToList();

            var actual = (from num in this.clientContext.Products
                         where num.ProductID < 5
                         select num).ToList();

            Assert.AreEqual(expected.Count(), actual.Count(), "Count of products where ids are lower then 5 did not return the correct number of values, expected {0} but the actual was {1}.", expected.Count(), actual.Count());

            Assert.IsTrue(actual.All(left => expected.Any(right => right.ProductID == left.ProductID)), "List of products where ids are lower then 5 are not identical");
        }

        [TestCategory("Restriction Operators")]
        [TestMethod]//[Title("Where_Simple 2")]
        [Description("This sample uses the where clause to find all products that are out of stock.")]
        public void Where_Simple_2()
        {
            var expected = from prod in this.repository.GetProductList()
                                  where prod.UnitsInStock == 0
                                  select prod;

            var actual = from prod in this.clientContext.Products
                                  where prod.UnitsInStock == 0
                                  select prod;

            Assert.AreEqual(expected.Count(), actual.Count(), "Count of sold out products did not the correct number of values, expected {0} but the actual was {1}.", expected.Count(), actual.Count());

            Assert.IsTrue(actual.ToList().All(left => expected.Any(right => right.ProductID == left.ProductID)), "List of sold out products are not identical");
        }

        [TestCategory("Restriction Operators")]
        [TestMethod]//[Title("Where_Simple 3")]
        [Description("This sample uses the where clause to find all products that are in stock and " +
                     "cost more than 3.00 per unit.")]
        public void Where_Simple_3()
        {
            var expected = from prod in this.repository.GetProductList()
                           where prod.UnitsInStock > 0 && prod.UnitPrice > 3.00M
                           select prod;
                
            var actual = from prod in this.clientContext.Products
                         where prod.UnitsInStock > 0 && prod.UnitPrice > 3.00M
                         select prod;
                
            Assert.AreEqual(expected.Count(), actual.Count(), "Count of In-stock products that cost more than 3.00 did not the correct number of values, expected {0} but the actual was {1}.", expected.Count(), actual.Count());

            Assert.IsTrue(actual.ToList().All(left => expected.Any(right => right.ProductID == left.ProductID)), "List of In-stock products that cost more than 3.00 are not identical");
        }

        [TestCategory("Restriction Operators")]
        [TestMethod]//[Title("Where_Drilldown")]
        [Description("This sample uses the where clause to find all customers in Washington " +
                     "and then it uses a foreach loop to iterate over the orders collection that belongs to each customer.")]
        public void Where_Drilldown()
        {
            var expected = (from cust in this.repository.GetCustomerList()
                           where cust.Region == "WA"
                           select cust).ToList();

            var actual = (from cust in this.clientContext.Customers
                         where cust.Region == "WA"
                         select cust).ToList();
                
            Assert.AreEqual(expected.Count(), actual.Count(), "Count of Customers from Washington and their orders did not the correct number of values, expected {0} but the actual was {1}.", expected.Count(), actual.Count());

            Assert.IsTrue(actual.All(left =>
                expected.Any(right => right.CustomerID == left.CustomerID && left.Orders.Count() == right.Orders.Count() &&
                left.Orders.ToList().TrueForAll(leftOrder => right.Orders.Any(rightOrder => rightOrder.OrderID == leftOrder.OrderID))
                )), "List of Customers from Washington and their orders are not identical");
        }

        [TestCategory("Restriction Operators")]
        [TestMethod]//[Title("Where_Indexed")]
        [Description("This sample demonstrates an indexed where clause that returns digits whose name is " +
                    "shorter than their value.")]
        public void Where_Indexed()
        {
            var expected = this.repository.GetListOfDigits().Where((digit, index) => digit.Length < index).ToList();

            var actual = this.clientContext.Digits.Where((digit, index) => digit.Length < index).ToList();

            Assert.AreEqual(expected.Count(), actual.Count(), "Count of digits whose name is shorter than their value did not the correct number of values, expected {0} but the actual was {1}.", expected.Count(), actual.Count());

            Assert.IsTrue(actual.All(left => expected.Any(right => string.Compare(left, right, false) == 0)), "List of digits whose name is shorter than their value are not identical");
        }

        [TestCategory("Projection Operators")]
        [TestMethod]//[Title("Select_Simple 1")]
        [Description("This sample uses the select clause to produce a sequence of ints one higher than " +
                     "those in an existing array of ints.")]
        public void Select_Simple_1()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var numsPlusOne =
                from num in numbers
                select num + 1;

            Console.WriteLine("Numbers + 1:");
            foreach (var i in numsPlusOne)
            {
                Console.WriteLine(i);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Projection Operators")]
        [TestMethod]//[Title("Select_Simple 2")]
        [Description("This sample uses the select clause to return a sequence of product names.")]
        public void Select_Simple_2()
        {
            var productNames =
                from prod in this.clientContext.Products
                select prod.ProductName;

            Console.WriteLine("Product Names:");
            foreach (var productName in productNames)
            {
                Console.WriteLine(productName);
            }
        }

        [TestCategory("Projection Operators")]
        [TestMethod]//[Title("Select_Transformation")]
        [Description("This sample uses the select clause to produce a sequence of strings representing " +
                     "the text version of a sequence of ints.")]
        public void Select_Transformation()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            string[] strings = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var textNums =
                from num in numbers
                select strings[num];

            Console.WriteLine("Number strings:");
            foreach (var str in textNums)
            {
                Console.WriteLine(str);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Projection Operators")]
        [TestMethod]//[Title("Select_Anonymous Types 1")]
        [Description("This sample uses the select clause to produce a sequence of the uppercase " +
                     "and lowercase versions of each word in the original array.")]
        public void Projection_Operators()
        {
            string[] words = { "aPPLE", "BlUeBeRrY", "cHeRry" };

            var upperLowerWords =
                from word in words
                select new { Upper = word.ToUpper(), Lower = word.ToLower() };

            foreach (var wordPair in upperLowerWords)
            {
                Console.WriteLine("Uppercase: {0}, Lowercase: {1}", wordPair.Upper, wordPair.Lower);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Projection Operators")]
        [TestMethod]//[Title("Select_Anonymous Types 2")]
        [Description("This sample uses the select clause to produce a sequence containing text " +
                     "representations of digits and a Boolean that specifies whether the text length is even or odd.")]
        public void Select_Anonymous_Types_2()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            string[] strings = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var digitOddEvens =
                from num in numbers
                select new { Digit = strings[num], Even = (num % 2 == 0) };

            foreach (var digit in digitOddEvens)
            {
                Console.WriteLine("The digit {0} is {1}.", digit.Digit, digit.Even ? "even" : "odd");
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Projection Operators")]
        [TestMethod]//[Title("Select_Anonymous Types 3")]
        [Description("This sample uses the select clause to produce a sequence containing some properties " +
                     "of Products, including UnitPrice which is renamed to Price " +
                     "in the resulting type.")]
        public void Select_Anonymous_Types_3()
        {
            var productInfos =
                from prod in this.clientContext.Products
                select new { prod.ProductName, prod.Category, Price = prod.UnitPrice };

            Console.WriteLine("Product Info:");
            foreach (var productInfo in productInfos)
            {
                Console.WriteLine("{0} is in the category {1} and costs {2} per unit.", productInfo.ProductName, productInfo.Category, productInfo.Price);
            }
        }

        [TestCategory("Projection Operators")]
        [TestMethod]//[Title("Select_Indexed")]
        [Description("This sample uses an indexed Select clause to determine if the value of ints " +
                     "in an array match their position in the array.")]
        public void Select_Indexed()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var numsInPlace = numbers.Select((num, index) => new { Num = num, InPlace = (num == index) });

            Console.WriteLine("Number: In-place?");
            foreach (var n in numsInPlace)
            {
                Console.WriteLine("{0}: {1}", n.Num, n.InPlace);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Projection Operators")]
        [TestMethod]//[Title("Select_Filtered")]
        [Description("This sample combines select and where to make a simple query that returns " +
                     "the text form of each digit less than 5.")]
        public void Select_Filtered()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            string[] digits = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var lowNums =
                from num in numbers
                where num < 5
                select digits[num];

            Console.WriteLine("Numbers < 5:");
            foreach (var num in lowNums)
            {
                Console.WriteLine(num);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Projection Operators")]
        [TestMethod]//[Title("SelectMany_Compound from 1")]
        [Description("This sample uses a compound from clause to make a query that returns all pairs " +
                     "of numbers from both arrays in which the number from numbersA is less than the number " +
                     "from numbersB.")]
        public void SelectMany_Compound_from_1()
        {
            int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
            int[] numbersB = { 1, 3, 5, 7, 8 };

            var pairs =
                from a in numbersA
                from b in numbersB
                where a < b
                select new { a, b };

            Console.WriteLine("Pairs where a < b:");
            foreach (var pair in pairs)
            {
                Console.WriteLine("{0} is less than {1}", pair.a, pair.b);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Projection Operators")]
        [TestMethod]//[Title("SelectMany_Compound from 2")]
        [Description("This sample uses a compound from clause to select all orders where the " +
                     "order total is less than 500.00.")]
        public void SelectMany_Compound_from_2()
        {
            var orders =
                from cust in this.clientContext.Customers
                from order in cust.Orders
                where order.Total < 500.00M
                select new { cust.CustomerID, order.OrderID, order.Total };

            ObjectDumper.Write(orders);
        }

        [TestCategory("Projection Operators")]
        [TestMethod]//[Title("SelectMany_Compound from 3")]
        [Description("This sample uses a compound from clause to select all orders where the " +
                     "order was made in 1998 or later.")]
        public void SelectMany_Compound_from_3()
        {
            var orders =
                from cust in this.clientContext.Customers
                from order in cust.Orders
                where order.OrderDate >= new DateTime(1998, 1, 1)
                select new { cust.CustomerID, order.OrderID, order.OrderDate };

            ObjectDumper.Write(orders);
        }

        [TestCategory("Projection Operators")]
        [TestMethod]//[Title("SelectMany_With let")]
        [Description("This sample uses a compound from clause to select all orders where the " +
                     "order total is greater than 2000.00 and uses a let clause to avoid " +
                     "requesting the total twice.")]
        public void SelectMany_With_let()
        {
            var orders =
                from cust in this.clientContext.Customers
                from order in cust.Orders
                let total = order.Total
                where total >= 2000.0M
                select new { cust.CustomerID, order.OrderID, total };

            ObjectDumper.Write(orders);
        }

        [TestCategory("Projection Operators")]
        [TestMethod]//[Title("SelectMany_Compound from")]
        [Description("This sample uses compound from clauses so that filtering on customers can " +
                     "be done before selecting their orders.  This makes the query more efficient by " +
                     "not selecting and then discarding orders for customers outside of Washington.")]
        public void SelectMany_Compound_from()
        {
            DateTime cutoffDate = new DateTime(1997, 1, 1);

            var orders =
                from cust in this.clientContext.Customers
                where cust.Region == "WA"
                from order in cust.Orders
                where order.OrderDate >= cutoffDate
                select new { cust.CustomerID, order.OrderID };

            ObjectDumper.Write(orders);
        }

        [TestCategory("Projection Operators")]
        [TestMethod]//[Title("SelectMany_Indexed")]
        [Description("This sample uses an indexed SelectMany clause to select all orders, " +
                     "while referring to customers by the order in which they are returned " +
                     "from the query.")]
        public void SelectMany_Indexed()
        {
            var customerOrders =
                this.clientContext.Customers.SelectMany(
                    (cust, custIndex) =>
                    cust.Orders.Select(o => "Customer #" + (custIndex + 1) +
                                            " has an order with OrderID " + o.OrderID));

            ObjectDumper.Write(customerOrders);
        }

        [TestCategory("Partitioning Operators")]
        [TestMethod]//[Title("Take_Simple")]
        [Description("This sample uses Take to get only the first 3 elements of " +
                     "the array.")]
        public void Take_Simple()
        {
            var first3Numbers =  this.clientContext.Digits.Take(3);

            Console.WriteLine("First 3 digits:");
            foreach (var n in first3Numbers)
            {
                Console.WriteLine(n);
            }
        }

        [TestCategory("Partitioning Operators")]
        [TestMethod]//[Title("Take_Nested")]
        [Description("This sample uses Take to get the first 3 orders from customers " +
                     "in Washington.")]
        public void Take_Nested()
        {
            var first3WAOrders = (
                from cust in this.clientContext.Customers
                from order in cust.Orders
                where cust.Region == "WA"
                select new { cust.CustomerID, order.OrderID, order.OrderDate })
                .Take(3);

            Console.WriteLine("First 3 orders in WA:");
            foreach (var order in first3WAOrders)
            {
                ObjectDumper.Write(order);
            }
        }

        [TestCategory("Partitioning Operators")]
        [TestMethod]//[Title("Skip_Simple")]
        [Description("This sample uses Skip to get all but the first four elements of " +
                     "the array.")]
        public void Skip_Simple()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var allButFirst4Numbers = numbers.Skip(4);

            Console.WriteLine("All but first 4 numbers:");
            foreach (var n in allButFirst4Numbers)
            {
                Console.WriteLine(n);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Partitioning Operators")]
        [TestMethod]//[Title("Skip_Nested")]
        [Description("This sample uses Take to get all but the first 2 orders from customers " +
                     "in Washington.")]
        public void Skip_Nested()
        {
            var waOrders =
                from cust in this.clientContext.Customers
                from order in cust.Orders
                where cust.Region == "WA"
                select new { cust.CustomerID, order.OrderID, order.OrderDate };

            var allButFirst2Orders = waOrders.Skip(2);

            Console.WriteLine("All but first 2 orders in WA:");
            foreach (var order in allButFirst2Orders)
            {
                ObjectDumper.Write(order);
            }
        }

        [TestCategory("Partitioning Operators")]
        [TestMethod]//[Title("TakeWhile_Simple")]
        [Description("This sample uses TakeWhile to return elements starting from the " +
                     "beginning of the array until a number is read whose value is not less than 6.")]
        public void TakeWhile_Simple()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var firstNumbersLessThan6 = numbers.TakeWhile(n => n < 6);

            Console.WriteLine("First numbers less than 6:");
            foreach (var num in firstNumbersLessThan6)
            {
                Console.WriteLine(num);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Partitioning Operators")]
        [TestMethod]//[Title("TakeWhile_Indexed")]
        [Description("This sample uses TakeWhile to return elements starting from the " +
                    "beginning of the array until a number is hit that is less than its position " +
                    "in the array.")]
        public void TakeWhile_Indexed()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var firstSmallNumbers = numbers.TakeWhile((n, index) => n >= index);

            Console.WriteLine("First numbers not less than their position:");
            foreach (var n in firstSmallNumbers)
            {
                Console.WriteLine(n);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Partitioning Operators")]
        [TestMethod]//[Title("SkipWhile_Simple")]
        [Description("This sample uses SkipWhile to get the elements of the array " +
                    "starting from the first element divisible by 3.")]
        public void SkipWhile_Simple()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            // In the lambda expression, 'n' is the input parameter that identifies each
            // element in the collection in succession. It is is inferred to be
            // of type int because numbers is an int array.
            var allButFirst3Numbers = numbers.SkipWhile(n => n % 3 != 0);

            Console.WriteLine("All elements starting from first element divisible by 3:");
            foreach (var n in allButFirst3Numbers)
            {
                Console.WriteLine(n);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Partitioning Operators")]
        [TestMethod]//[Title("SkipWhile_Indexed")]
        [Description("This sample uses SkipWhile to get the elements of the array " +
                    "starting from the first element less than its position.")]
        public void SkipWhile_Indexed()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var laterNumbers = numbers.SkipWhile((n, index) => n >= index);

            Console.WriteLine("All elements starting from first element less than its position:");
            foreach (var n in laterNumbers)
            {
                Console.WriteLine(n);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Ordering Operators")]
        [TestMethod]//[Title("OrderBy_Simple 1")]
        [Description("This sample uses orderby to sort a list of words alphabetically.")]
        public void OrderBy_Simple_1()
        {
            string[] words = { "cherry", "apple", "blueberry" };

            var sortedWords =
                from word in words
                orderby word
                select word;

            Console.WriteLine("The sorted list of words:");
            foreach (var w in sortedWords)
            {
                Console.WriteLine(w);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Ordering Operators")]
        [TestMethod]//[Title("OrderBy_Simple 2")]
        [Description("This sample uses orderby to sort a list of words by length.")]
        public void OrderBy_Simple_2()
        {
            string[] words = { "cherry", "apple", "blueberry" };

            var sortedWords =
                from word in words
                orderby word.Length
                select word;

            Console.WriteLine("The sorted list of words (by length):");
            foreach (var w in sortedWords)
            {
                Console.WriteLine(w);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Ordering Operators")]
        [TestMethod]//[Title("OrderBy_Simple 3")]
        [Description("This sample uses orderby to sort a list of products by name. " +
                    "Use the \"descending\" keyword at the end of the clause to perform a reverse ordering.")]
        public void OrderBy_Simple_3()
        {
            var sortedProducts =
                from prod in this.clientContext.Products
                orderby prod.ProductName
                select prod;

            ObjectDumper.Write(sortedProducts);
        }

        // Custom comparer for use with ordering operators
        public class CaseInsensitiveComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
            }
        }

        [TestCategory("Ordering Operators")]
        [TestMethod]//[Title("OrderBy_Comparer")]
        [Description("This sample uses an OrderBy clause with a custom comparer to " +
                     "do a case-insensitive sort of the words in an array.")]
        public void OrderBy_Comparer()
        {
            string[] words = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry" };

            var sortedWords = words.OrderBy(a => a, new CaseInsensitiveComparer());

            ObjectDumper.Write(sortedWords);

            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Ordering Operators")]
        [TestMethod]//[Title("OrderByDescending_Simple 1")]
        [Description("This sample uses orderby and descending to sort a list of " +
                     "doubles from highest to lowest.")]
        public void OrderByDescending_Simple_1()
        {
            double[] doubles = { 1.7, 2.3, 1.9, 4.1, 2.9 };

            var sortedDoubles =
                from d in doubles
                orderby d descending
                select d;

            Console.WriteLine("The doubles from highest to lowest:");
            foreach (var d in sortedDoubles)
            {
                Console.WriteLine(d);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Ordering Operators")]
        [TestMethod]//[Title("OrderByDescending_Simple 2")]
        [Description("This sample uses orderby to sort a list of products by units in stock " +
                     "from highest to lowest.")]
        public void OrderByDescending_Simple_2()
        {
            var sortedProducts =
                from prod in this.clientContext.Products
                orderby prod.UnitsInStock descending
                select prod;

            ObjectDumper.Write(sortedProducts);
        }

        [TestCategory("Ordering Operators")]
        [TestMethod]//[Title("OrderByDescending_Comparer")]
        [Description("This sample uses method syntax to call OrderByDescending because it " +
                    " enables you to use a custom comparer.")]
        public void OrderByDescending_Comparer()
        {
            string[] words = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry" };

            var sortedWords = words.OrderByDescending(a => a, new CaseInsensitiveComparer());

            ObjectDumper.Write(sortedWords);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Ordering Operators")]
        [TestMethod]//[Title("ThenBy_Simple")]
        [Description("This sample uses a compound orderby to sort a list of digits, " +
                     "first by length of their name, and then alphabetically by the name itself.")]
        public void ThenBy_Simple()
        {
            string[] digits = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            var sortedDigits =
                from digit in this.clientContext.Digits
                orderby digit.Length, digit
                select digit;

            Console.WriteLine("Sorted digits:");
            foreach (var d in sortedDigits)
            {
                Console.WriteLine(d);
            }
        }

        [TestCategory("Ordering Operators")]
        [TestMethod]//[Title("ThenBy_Comparer")]
        [Description("The first query in this sample uses method syntax to call OrderBy and ThenBy with a custom comparer to " +
                     "sort first by word length and then by a case-insensitive sort of the words in an array. " +
                     "The second two queries show another way to perform the same task.")]
        public void ThenBy_Comparer()
        {
            string[] words = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry" };

            var sortedWords =
                words.OrderBy(a => a.Length)
                     .ThenBy(a => a, new CaseInsensitiveComparer());

            // Another way. TODO is this use of ThenBy correct? It seems to work on this sample array.
            var sortedWords2 =
                from word in words
                orderby word.Length
                select word;

            var sortedWords3 = sortedWords2.ThenBy(a => a, new CaseInsensitiveComparer());

            ObjectDumper.Write(sortedWords);

            ObjectDumper.Write(sortedWords3);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Ordering Operators")]
        [TestMethod]//[Title("ThenByDescending_Simple")]
        [Description("This sample uses a compound orderby to sort a list of products, " +
                     "first by category, and then by unit price, from highest to lowest.")]
        public void ThenByDescending_Simple()
        {
            var sortedProducts =
                from prod in this.clientContext.Products
                orderby prod.Category, prod.UnitPrice descending
                select prod;

            ObjectDumper.Write(sortedProducts);
        }

        [TestCategory("Ordering Operators")]
        [TestMethod]//[Title("ThenByDescending_Comparer")]
        [Description("This sample uses an OrderBy and a ThenBy clause with a custom comparer to " +
                     "sort first by word length and then by a case-insensitive descending sort " +
                     "of the words in an array.")]
        public void ThenByDescending_Comparer()
        {
            string[] words = { "aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry" };

            var sortedWords =
                words.OrderBy(a => a.Length)
                     .ThenByDescending(a => a, new CaseInsensitiveComparer());

            ObjectDumper.Write(sortedWords);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Ordering Operators")]
        [TestMethod]//[Title("Reverse")]
        [Description("This sample uses Reverse to create a list of all digits in the array whose " +
                     "second letter is 'i' that is reversed from the order in the original array.")]
        public void Reverse()
        {
            var reversedIDigits = (
                from digit in this.clientContext.Digits
                where digit[1] == 'i'
                select digit)
                .Reverse();

            Console.WriteLine("A backwards list of the digits with a second character of 'i':");
            foreach (var d in reversedIDigits)
            {
                Console.WriteLine(d);
            }
        }

        [TestCategory("Grouping Operators")]
        [TestMethod]//[Title("GroupBy_Simple 1")]
        [Description("This sample uses group by to partition a list of numbers by " +
                    "their remainder when divided by 5.")]
        public void GroupBy_Simple_1()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var numberGroups =
                from num in numbers
                group num by num % 5 into numGroup
                select new { Remainder = numGroup.Key, Numbers = numGroup };

            foreach (var grp in numberGroups)
            {
                Console.WriteLine("Numbers with a remainder of {0} when divided by 5:", grp.Remainder);
                foreach (var n in grp.Numbers)
                {
                    Console.WriteLine(n);
                }
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Grouping Operators")]
        [TestMethod]//[Title("GroupBy_Simple 2")]
        [Description("This sample uses group by to partition a list of words by " +
                     "their first letter.")]
        public void GroupBy_Simple_2()
        {
            string[] words = { "blueberry", "chimpanzee", "abacus", "banana", "apple", "cheese" };

            var wordGroups =
                from num in words
                group num by num[0] into grp
                select new { FirstLetter = grp.Key, Words = grp };

            foreach (var wordgrp in wordGroups)
            {
                Console.WriteLine("Words that start with the letter '{0}':", wordgrp.FirstLetter);
                foreach (var word in wordgrp.Words)
                {
                    Console.WriteLine(word);
                }
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Grouping Operators")]
        [TestMethod]//[Title("GroupBy_Simple 3")]
        [Description("This sample uses group by to partition a list of products by category.")]
        public void GroupBy_Simple_3()
        {
            var orderGroups =
                from prod in this.clientContext.Products
                group prod by prod.Category into prodGroup
                select new { Category = prodGroup.Key, Products = prodGroup };

            ObjectDumper.Write(orderGroups, 1);
        }

        [TestCategory("Grouping Operators")]
        [TestMethod]//[Title("GroupBy_Nested")]
        [Description("This sample uses group by to partition a list of each customer's orders, " +
                     "first by year, and then by month.")]
        public void GroupBy_Nested()
        {
            var customerOrderGroups =
                from cust in this.clientContext.Customers
                select
                    new
                    {
                        cust.CompanyName,
                        YearGroups =
                            from order in cust.Orders
                            group order by order.OrderDate.Year into yearGroup
                            select
                                new
                                {
                                    Year = yearGroup.Key,
                                    MonthGroups =
                                        from order in yearGroup
                                        group order by order.OrderDate.Month into MonthGroup
                                        select new { Month = MonthGroup.Key, Orders = MonthGroup }
                                }
                    };

            ObjectDumper.Write(customerOrderGroups, 3);
        }

        public class AnagramEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return getCanonicalString(x) == getCanonicalString(y);
            }

            public int GetHashCode(string obj)
            {
                return getCanonicalString(obj).GetHashCode();
            }

            private string getCanonicalString(string word)
            {
                char[] wordChars = word.ToCharArray();
                Array.Sort<char>(wordChars);
                return new string(wordChars);
            }
        }

        [TestCategory("Grouping Operators")]
        [TestMethod]//[Title("GroupBy_Comparer")]
        [Description("This sample uses GroupBy with method syntax to partition trimmed elements of an array using " +
                     "a custom comparer that matches words that are anagrams of each other.")]
        public void GroupBy_Comparer()
        {
            string[] anagrams = { "from   ", " salt", " earn ", "  last   ", " near ", " form  " };

            var orderGroups = anagrams.GroupBy(w => w.Trim(), new AnagramEqualityComparer());

            ObjectDumper.Write(orderGroups, 1);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Grouping Operators")]
        [TestMethod]//[Title("GroupBy_Comparer, Mapped")]
        [Description("This sample uses the GroupBy method to partition trimmed elements of an array using " +
                     "a custom comparer that matches words that are anagrams of each other, " +
                     "and then converts the results to uppercase.")]
        public void GroupBy_Comparer_Mapped()
        {
            string[] anagrams = { "from   ", " salt", " earn ", "  last   ", " near ", " form  " };

            var orderGroups = anagrams.GroupBy(
                        w => w.Trim(),
                        a => a.ToUpper(),
                        new AnagramEqualityComparer()
                        );

            ObjectDumper.Write(orderGroups, 1);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Set Operators")]
        [TestMethod]//[Title("Distinct_1")]
        [Description("This sample uses Distinct to remove duplicate elements in a sequence of " +
                    "factors of 300.")]
        public void Distinct_1()
        {
            int[] factorsOf300 = { 2, 2, 3, 5, 5 };

            var uniqueFactors = factorsOf300.Distinct();

            Console.WriteLine("Prime factors of 300:");
            foreach (var f in uniqueFactors)
            {
                Console.WriteLine(f);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Set Operators")]
        [TestMethod]//[Title("Distinct_2")]
        [Description("This sample uses Distinct to find the unique Category names.")]
        public void Distinct_2()
        {
            var categoryNames = (
                from prod in this.clientContext.Products
                select prod.Category)
                .Distinct();

            Console.WriteLine("Category names:");
            foreach (var n in categoryNames)
            {
                Console.WriteLine(n);
            }
        }

        [TestCategory("Set Operators")]
        [TestMethod]//[Title("Union_1")]
        [Description("This sample uses Union to create one sequence that contains the unique values " +
                     "from both arrays.")]
        public void Union_1()
        {
            int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
            int[] numbersB = { 1, 3, 5, 7, 8 };

            var uniqueNumbers = numbersA.Union(numbersB);

            Console.WriteLine("Unique numbers from both arrays:");
            foreach (var n in uniqueNumbers)
            {
                Console.WriteLine(n);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Set Operators")]
        [TestMethod]//[Title("Union_2")]
        [Description("This sample uses the Union method to create one sequence that contains the unique first letter " +
                     "from both product and customer names. Union is only available through method syntax.")]
        public void Union_2()
        {
            var productFirstChars =
                from prod in this.clientContext.Products
                select prod.ProductName[0];

            var customerFirstChars =
                from cust in this.clientContext.Customers
                select cust.CompanyName[0];

            var uniqueFirstChars = productFirstChars.Union(customerFirstChars);

            Console.WriteLine("Unique first letters from Product names and Customer names:");
            foreach (var ch in uniqueFirstChars)
            {
                Console.WriteLine(ch);
            }
        }

        [TestCategory("Set Operators")]
        [TestMethod]//[Title("Intersect_1")]
        [Description("This sample uses Intersect to create one sequence that contains the common values " +
                    "shared by both arrays.")]
        public void Intersect_1()
        {
            int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
            int[] numbersB = { 1, 3, 5, 7, 8 };

            var commonNumbers = numbersA.Intersect(numbersB);

            Console.WriteLine("Common numbers shared by both arrays:");
            foreach (var n in commonNumbers)
            {
                Console.WriteLine(n);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Set Operators")]
        [TestMethod]//[Title("Intersect_2")]
        [Description("This sample uses Intersect to create one sequence that contains the common first letter " +
                     "from both product and customer names.")]
        public void Intersect_2()
        {
            var productFirstChars =
                from prod in this.clientContext.Products
                select prod.ProductName[0];
            var customerFirstChars =
                from cust in this.clientContext.Customers
                select cust.CompanyName[0];

            var commonFirstChars = productFirstChars.Intersect(customerFirstChars);

            Console.WriteLine("Common first letters from Product names and Customer names:");
            foreach (var ch in commonFirstChars)
            {
                Console.WriteLine(ch);
            }
        }

        [TestCategory("Set Operators")]
        [TestMethod]//[Title("Except_1")]
        [Description("This sample uses Except to create a sequence that contains the values from numbersA" +
                     "that are not also in numbersB.")]
        public void Except_1()
        {
            int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
            int[] numbersB = { 1, 3, 5, 7, 8 };

            IEnumerable<int> aOnlyNumbers = numbersA.Except(numbersB);

            Console.WriteLine("Numbers in first array but not second array:");
            foreach (var n in aOnlyNumbers)
            {
                Console.WriteLine(n);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Set Operators")]
        [TestMethod]//[Title("Except_2")]
        [Description("This sample uses Except to create one sequence that contains the first letters " +
                     "of product names that are not also first letters of customer names.")]
        public void Except_2()
        {
            var productFirstChars =
                from prod in this.clientContext.Products
                select prod.ProductName[0];
            var customerFirstChars =
                from cust in this.clientContext.Customers
                select cust.CompanyName[0];

            var productOnlyFirstChars = productFirstChars.Except(customerFirstChars);

            Console.WriteLine("First letters from Product names, but not from Customer names:");
            foreach (var ch in productOnlyFirstChars)
            {
                Console.WriteLine(ch);
            }
        }

        [TestCategory("Conversion Operators")]
        [TestMethod]//[Title("ToArray")]
        [Description("This sample uses ToArray to immediately evaluate a sequence into an array.")]
        public void ToArray()
        {
            double[] doubles = { 1.7, 2.3, 1.9, 4.1, 2.9 };

            var sortedDoubles =
                from d in doubles
                orderby d descending
                select d;
            var doublesArray = sortedDoubles.ToArray();

            Console.WriteLine("Every other double from highest to lowest:");
            for (int d = 0; d < doublesArray.Length; d += 2)
            {
                Console.WriteLine(doublesArray[d]);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Conversion Operators")]
        [TestMethod]//[Title("ToList")]
        [Description("This sample uses ToList to immediately evaluate a sequence into a List<T>.")]
        public void ToList()
        {
            string[] words = { "cherry", "apple", "blueberry" };

            var sortedWords =
                from w in words
                orderby w
                select w;
            var wordList = sortedWords.ToList();

            Console.WriteLine("The sorted word list:");
            foreach (var w in wordList)
            {
                Console.WriteLine(w);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Conversion Operators")]
        [TestMethod]//[Title("ToDictionary")]
        [Description("This sample uses ToDictionary to immediately evaluate a sequence and a " +
                    "related key expression into a dictionary.")]
        public void ToDictionary()
        {
            var scoreRecords = new[] { new {Name = "Alice", Score = 50},
                                        new {Name = "Bob"  , Score = 40},
                                        new {Name = "Cathy", Score = 45}
                                    };

            var scoreRecordsDict = scoreRecords.ToDictionary(sr => sr.Name);

            Console.WriteLine("Bob's score: {0}", scoreRecordsDict["Bob"]);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Conversion Operators")]
        [TestMethod]//[Title("OfType")]
        [Description("This sample uses OfType to return only the elements of the array that are of type double.")]
        public void OfType()
        {
            object[] numbers = { null, 1.0, "two", 3, "four", 5, "six", 7.0 };

            var doubles = numbers.OfType<double>();

            Console.WriteLine("Numbers stored as doubles:");
            foreach (var d in doubles)
            {
                Console.WriteLine(d);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Element Operators")]
        [TestMethod]//[Title("First_Simple")]
        [Description("This sample uses First to return the first matching element " +
                     "as a Product, instead of as a sequence containing a Product.")]
        public void First_Simple()
        {
            Product product12 = (
                from prod in this.clientContext.Products
                where prod.ProductID == 12
                select prod)
                .First();

            ObjectDumper.Write(product12);
        }

        [TestCategory("Element Operators")]
        [TestMethod]//[Title("First_Condition")]
        [Description("This sample uses First to find the first element in the array that starts with 'o'.")]
        public void First_Condition()
        {
            Product product12 = this.clientContext.Products.First(prod => prod.ProductID == 12);

            ObjectDumper.Write(product12);
        }

        [TestCategory("Element Operators")]
        [TestMethod]//[Title("FirstOrDefault_Simple")]
        [Description("This sample uses FirstOrDefault to try to return the first element of the sequence, " +
                     "unless there are no elements, in which case the default value for that type " +
                     "is returned. FirstOrDefault is useful for creating outer joins.")]
        public void FirstOrDefault_Simple()
        {
            Product product = this.clientContext.Products.FirstOrDefault();

            Console.WriteLine("Product exists: {0}", product != null);
            //Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Element Operators")]
        [TestMethod]//[Title("FirstOrDefault_Condition")]
        [Description("This sample uses FirstOrDefault to return the first product whose ProductID is 789 " +
                     "as a single Product object, unless there is no match, in which case null is returned.")]
        public void FirstOrDefault_Condition()
        {
            Product product789 = this.clientContext.Products.FirstOrDefault(p => p.ProductID == 789);

            Console.WriteLine("Product 789 exists: {0}", product789 != null);
        }

        [TestCategory("Element Operators")]
        [TestMethod]//[Title("ElementAt")]
        [Description("This sample uses ElementAt to retrieve the second number greater than 5 " +
                     "from an array.")]
        public void ElementAt()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            int fourthLowNum = (
                from num in numbers
                where num > 5
                select num)
                .ElementAt(1);  // second number is index 1 because sequences use 0-based indexing

            Console.WriteLine("Second number > 5: {0}", fourthLowNum);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Generation Operators")]
        [TestMethod]//[Title("Range")]
        [Description("This sample uses Range to generate a sequence of numbers from 100 to 149 " +
                     "that is used to find which numbers in that range are odd and even.")]
        public void Range()
        {
            var numbers =
                from n in Enumerable.Range(100, 50)
                select new { Number = n, OddEven = n % 2 == 1 ? "odd" : "even" };

            foreach (var n in numbers)
            {
                Console.WriteLine("The number {0} is {1}.", n.Number, n.OddEven);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Generation Operators")]
        [TestMethod]//[Title("Repeat")]
        [Description("This sample uses Repeat to generate a sequence that contains the number 7 ten times.")]
        public void Repeat()
        {
            var numbers = Enumerable.Repeat(7, 10);

            foreach (var n in numbers)
            {
                Console.WriteLine(n);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }


        [TestCategory("Quantifiers")]
        [TestMethod]//[Title("Any_Simple")]
        [Description("This sample uses Any to determine if any of the words in the array " +
                     "contain the substring 'ei'.")]
        public void Any_Simple()
        {
            string[] words = { "believe", "relief", "receipt", "field" };

            bool iAfterE = words.Any(w => w.Contains("ei"));

            //DONE fixed typo in writeline
            Console.WriteLine("There is a word in the list that contains 'ei': {0}", iAfterE);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Quantifiers")]
        [TestMethod]//[Title("Any_Grouped")]
        [Description("This sample uses Any to return a grouped a list of products only for categories " +
                     "that have at least one product that is out of stock.")]
        public void Any_Grouped()
        {
            var productGroups =
                from prod in this.clientContext.Products
                group prod by prod.Category into prodGroup
                where prodGroup.Any(p => p.UnitsInStock == 0)
                select new { Category = prodGroup.Key, Products = prodGroup };

            ObjectDumper.Write(productGroups, 1);
        }

        [TestCategory("Quantifiers")]
        [TestMethod]//[Title("All_Simple")]
        [Description("This sample uses All to determine whether an array contains " +
                     "only odd numbers.")]
        public void All_Simple()
        {
            int[] numbers = { 1, 11, 3, 19, 41, 65, 19 };

            bool onlyOdd = numbers.All(n => n % 2 == 1);

            Console.WriteLine("The list contains only odd numbers: {0}", onlyOdd);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Quantifiers")]
        [TestMethod]//[Title("All_Grouped")]
        [Description("This sample uses All to return a grouped a list of products only for categories " +
                     "that have all of their products in stock.")]
        public void All_Grouped()
        {
            var productGroups =
                from prod in this.clientContext.Products
                group prod by prod.Category into prodGroup
                where prodGroup.All(p => p.UnitsInStock > 0)
                select new { Category = prodGroup.Key, Products = prodGroup };

            ObjectDumper.Write(productGroups, 1);
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Count_Simple")]
        [Description("This sample uses Count to get the number of unique prime factors of 300.")]
        public void Count_Simple()
        {
            int[] primeFactorsOf300 = { 2, 2, 3, 5, 5 };

            int uniqueFactors = this.clientContext.Products.Count();

            Console.WriteLine("There are {0} unique prime factors of 300.", uniqueFactors);
            //Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Count_Conditional")]
        [Description("This sample uses Count to get the number of odd ints in the array.")]
        public void Count_Conditional()
        {
            var expected = (from product in this.repository.GetProductList()
                            select product.ProductID).Count(n => n % 2 == 1);

            var actual = (from product in this.clientContext.Products
                          select product.ProductID).Count(n => n % 2 == 1);

            Assert.AreEqual(expected, actual, "Getting the number of odd ints in the array did not return values, expected {0} but the actual was {1}.", expected, actual);
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Count_Nested")]
        [Description("This sample uses Count to return a list of customers and how many orders " +
                     "each has.")]
        public void Count_Nested()
        {
            var orderCounts =
                from cust in this.clientContext.Customers
                select new { cust.CustomerID, OrderCount = cust.Orders.Count() };

            ObjectDumper.Write(orderCounts);
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Count_Grouped")]
        [Description("This sample uses Count to return a list of categories and how many products " +
                     "each has.")]
        public void Count_Grouped()
        {
            var categoryCounts =
                from prod in this.clientContext.Products
                group prod by prod.Category into prodGroup
                select new { Category = prodGroup.Key, ProductCount = prodGroup.Count() };

            ObjectDumper.Write(categoryCounts);
        }

        //DONE Changed "get the total of" to "add all"
        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Sum_Simple")]
        [Description("This sample uses Sum to add all the numbers in an array.")]
        public void Sum_Simple()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            double numSum = numbers.Sum();

            Console.WriteLine("The sum of the numbers is {0}.", numSum);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Sum_Projection")]
        [Description("This sample uses Sum to get the total number of characters of all words " +
                     "in the array.")]
        public void Sum_Projection()
        {
            double totalUnits = this.clientContext.Products.Sum(w => w.UnitsInStock);

            Console.WriteLine("There are a total of {0} units in stock.", totalUnits);
        }



        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Sum_Grouped")]
        [Description("This sample uses Sum to get the total units in stock for each product category.")]
        public void Sum_Grouped()
        {
            var categories =
                from prod in this.clientContext.Products
                group prod by prod.Category into prodGroup
                select new { Category = prodGroup.Key, TotalUnitsInStock = prodGroup.Sum(p => p.UnitsInStock) };

            ObjectDumper.Write(categories);
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Min_Simple")]
        [Description("This sample uses Min to get the lowest number in an array.")]
        public void Min_Simple()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            int minNum = numbers.Min();

            Console.WriteLine("The minimum number is {0}.", minNum);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Min_Projection")]
        [Description("This sample uses Min to get the length of the shortest word in an array.")]
        public void Min_Projection()
        {
            string[] words = { "cherry", "apple", "blueberry" };

            int shortestWord = words.Min(w => w.Length);

            Console.WriteLine("The shortest word is {0} characters long.", shortestWord);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Min_Grouped")]
        [Description("This sample uses Min to get the cheapest price among each category's products.")]
        public void Min_Grouped()
        {
            var categories =
                from prod in this.clientContext.Products
                group prod by prod.Category into prodGroup
                select new { Category = prodGroup.Key, CheapestPrice = prodGroup.Min(p => p.UnitPrice) };

            ObjectDumper.Write(categories);
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Min_Elements")]
        [Description("This sample uses Min to get the products with the lowest price in each category.")]
        public void Min_Elements()
        {
            var categories =
                from prod in this.clientContext.Products
                group prod by prod.Category into prodGroup
                let minPrice = prodGroup.Min(p => p.UnitPrice)
                select new { Category = prodGroup.Key, CheapestProducts = prodGroup.Where(p => p.UnitPrice == minPrice) };

            ObjectDumper.Write(categories, 1);
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Max_Simple")]
        [Description("This sample uses Max to get the highest number in an array. Note that the method returns a single value.")]
        public void Max_Simple()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            int maxNum = numbers.Max();

            Console.WriteLine("The maximum number is {0}.", maxNum);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Max_Projection")]
        [Description("This sample uses Max to get the length of the longest word in an array.")]
        public void Max_Projection()
        {
            string[] words = { "cherry", "apple", "blueberry" };

            int longestLength = words.Max(w => w.Length);

            Console.WriteLine("The longest word is {0} characters long.", longestLength);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Max_Grouped")]
        [Description("This sample uses Max to get the most expensive price among each category's products.")]
        public void Max_Grouped()
        {
            var categories =
                from prod in this.clientContext.Products
                group prod by prod.Category into prodGroup
                select new { Category = prodGroup.Key, MostExpensivePrice = prodGroup.Max(p => p.UnitPrice) };

            ObjectDumper.Write(categories);
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Max_Elements")]
        [Description("This sample uses Max to get the products with the most expensive price in each category.")]
        public void Max_Elements()
        {
            var categories =
                from prod in this.clientContext.Products
                group prod by prod.Category into prodGroup
                let maxPrice = prodGroup.Max(p => p.UnitPrice)
                select new { Category = prodGroup.Key, MostExpensiveProducts = prodGroup.Where(p => p.UnitPrice == maxPrice) };

            ObjectDumper.Write(categories, 1);
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Average_Simple")]
        [Description("This sample uses Average to get the average of all numbers in an array.")]
        public void Average_Simple()
        {
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            decimal averageNum = this.clientContext.Products.Select(p => p.UnitPrice).Average();

            Console.WriteLine("The average number is {0}.", averageNum);
            //Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Average_Projection")]
        [Description("This sample uses Average to get the average length of the words in the array.")]
        public void Average_Projection()
        {
            string[] words = { "cherry", "apple", "blueberry" };

            double averageLength = words.Average(w => w.Length);

            Console.WriteLine("The average word length is {0} characters.", averageLength);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Average_Grouped")]
        [Description("This sample uses Average to get the average price of each category's products.")]
        public void Average_Grouped()
        {
            var categories =
                from prod in this.clientContext.Products
                group prod by prod.Category into prodGroup
                select new { Category = prodGroup.Key, AveragePrice = prodGroup.Average(p => p.UnitPrice) };

            ObjectDumper.Write(categories);
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Aggregate_Simple")]
        [Description("This sample uses Aggregate to create a running product on the array that " +
                     "calculates the total product of all elements.")]
        public void Aggregate_Simple()
        {
            double[] doubles = { 1.7, 2.3, 1.9, 4.1, 2.9 };

            double product = doubles.Aggregate((runningProduct, nextFactor) => runningProduct * nextFactor);

            Console.WriteLine("Total product of all numbers: {0}", product);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Aggregate Operators")]
        [TestMethod]//[Title("Aggregate_Seed")]
        [Description("This sample uses Aggregate to create a running account balance that " +
                     "subtracts each withdrawal from the initial balance of 100, as long as " +
                     "the balance never drops below 0.")]
        public void Aggregate_Seed()
        {
            double startBalance = 100.0;

            int[] attemptedWithdrawals = { 20, 10, 40, 50, 10, 70, 30 };

            double endBalance =
                attemptedWithdrawals.Aggregate(startBalance,
                    (balance, nextWithdrawal) =>
                        ((nextWithdrawal <= balance) ? (balance - nextWithdrawal) : balance));

            Console.WriteLine("Ending balance: {0}", endBalance);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Miscellaneous Operators")]
        [TestMethod]//[Title("Concat_1")]
        [Description("This sample uses Concat to create one sequence that contains each array's " +
                     "values, one after the other.")]
        public void Concat_1()
        {
            int[] numbersA = { 0, 2, 4, 5, 6, 8, 9 };
            int[] numbersB = { 1, 3, 5, 7, 8 };

            var allNumbers = numbersA.Concat(numbersB);

            Console.WriteLine("All numbers from both arrays:");
            foreach (var n in allNumbers)
            {
                Console.WriteLine(n);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Miscellaneous Operators")]
        [TestMethod]//[Title("Concat_2")]
        [Description("This sample uses Concat to create one sequence that contains the names of " +
                     "all customers and products, including any duplicates.")]
        public void Concat_2()
        {
            var customerNames =
                from cust in this.clientContext.Customers
                select cust.CompanyName;
            var productNames =
                from prod in this.clientContext.Products
                select prod.ProductName;

            var allNames = customerNames.Concat(productNames);

            Console.WriteLine("Customer and product names:");
            foreach (var n in allNames)
            {
                Console.WriteLine(n);
            }
        }

        [TestCategory("Miscellaneous Operators")]
        [TestMethod]//[Title("EqualAll_1")]
        [Description("This sample uses SequenceEquals to see if two sequences match on all elements " +
                     "in the same order.")]
        public void EqualAll_1()
        {
            var wordsA = new string[] { "cherry", "apple", "blueberry" };
            var wordsB = new string[] { "cherry", "apple", "blueberry" };

            bool match = wordsA.SequenceEqual(wordsB);

            Console.WriteLine("The sequences match: {0}", match);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Miscellaneous Operators")]
        [TestMethod]//[Title("EqualAll_2")]
        [Description("This sample uses SequenceEqual to see if two sequences match on all elements " +
                     "in the same order.")]
        public void EqualAll_2()
        {
            var wordsA = new string[] { "cherry", "apple", "blueberry" };
            var wordsB = new string[] { "apple", "blueberry", "cherry" };

            bool match = wordsA.SequenceEqual(wordsB);

            Console.WriteLine("The sequences match: {0}", match);
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Query Execution")]
        [TestMethod]//[Title("Deferred_Execution")]
        [Description("The following sample shows how query execution is deferred until the query is " +
                     "enumerated at a foreach statement.")]
        public void Deferred_Execution()
        {

            // Queries are not executed until you enumerate over them.
            int[] numbers = new int[] { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            int i = 0;
            var simpleQuery =
                from num in numbers
                select ++i;

            // The local variable 'i' is not incremented
            // until the query is executed in the foreach loop.
            Console.WriteLine("The current value of i is {0}", i); //i is still zero

            foreach (var item in simpleQuery)
            {
                Console.WriteLine("v = {0}, i = {1}", item, i); // now i is incremented          
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Query Execution")]
        [TestMethod]//[Title("Immediate_Execution")]
        [Description("The following sample shows how queries can be executed immediately, and their results " +
                    " stored in memory, with methods such as ToList.")]
        public void Immediate_Execution()
        {

            // Methods like ToList(), Max(), and Count() cause the query to be
            // executed immediately.            
            int[] numbers = new int[] { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            int i = 0;
            var immediateQuery = (
                from num in numbers
                select ++i)
                .ToList();

            Console.WriteLine("The current value of i is {0}", i); //i has been incremented

            foreach (var item in immediateQuery)
            {
                Console.WriteLine("v = {0}, i = {1}", item, i);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Query Execution")]
        [TestMethod]//[Title("Query_Reuse")]
        [Description("The following sample shows how, because of deferred execution, queries can be used " +
                     "again after data changes and will then operate on the new data.")]
        public void Query_Reuse()
        {

            // Deferred execution lets us define a query once
            // and then reuse it later in various ways.
            int[] numbers = new int[] { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            var lowNumbers =
                from num in numbers
                where num <= 3
                select num;

            Console.WriteLine("First run numbers <= 3:");
            foreach (int n in lowNumbers)
            {
                Console.WriteLine(n);
            }

            // Query the original query.
            var lowEvenNumbers =
                from num in lowNumbers
                where num % 2 == 0
                select num;

            Console.WriteLine("Run lowEvenNumbers query:");
            foreach (int n in lowEvenNumbers)
            {
                Console.WriteLine(n);
            }

            // Modify the source data.
            for (int i = 0; i < 10; i++)
            {
                numbers[i] = -numbers[i];
            }

            // During this second run, the same query object,
            // lowNumbers, will be iterating over the new state
            // of numbers[], producing different results:
            Console.WriteLine("Second run numbers <= 3:");
            foreach (int n in lowNumbers)
            {
                Console.WriteLine(n);
            }
            Assert.Inconclusive();
            //throw new NotImplementedException();
        }

        [TestCategory("Join Operators")]
        [TestMethod]//[Title("Inner_Join")]
        [Description("This sample shows how to perform a simple inner equijoin of two sequences to " +
            "to produce a flat result set that consists of each element in suppliers that has a matching element " +
            "in customers.")]
        public void Inner_Join()
        {
            var custSupJoin =
                from sup in this.clientContext.Suppliers
                join cust in this.clientContext.Customers on sup.Country equals cust.Country
                select new { Country = sup.Country, SupplierName = sup.SupplierName, CustomerName = cust.CompanyName };

            foreach (var item in custSupJoin)
            {
                Console.WriteLine("Country = {0}, Supplier = {1}, Customer = {2}", item.Country, item.SupplierName, item.CustomerName);
            }
        }

        [TestCategory("Join Operators")]
        [TestMethod]//[Title("Group_Join")]
        [Description("A group join produces a hierarchical sequence. The following query is an inner join " +
                    " that produces a sequence of objects, each of which has a key and an inner sequence of all matching elements.")]
        public void Group_Join()
        {
            var custSupQuery =
                from sup in this.clientContext.Suppliers
                join cust in this.clientContext.Customers on sup.Country equals cust.Country into cs
                select new { Key = sup.Country, Items = cs };


            foreach (var item in custSupQuery)
            {
                Console.WriteLine(item.Key + ":");
                foreach (var element in item.Items)
                {
                    Console.WriteLine("   " + element.CompanyName);
                }
            }
        }

        [TestCategory("Join Operators")]
        [TestMethod]//[Title("Cross_Join_with_Group_Join")]
        [Description("The group join operator is more general than join, as this slightly more verbose " +
            "version of the cross join sample shows.")]
        public void Cross_Join_with_Group_Join()
        {
            string[] categories = new string[]{ 
                "Beverages", 
                "Condiments", 
                "Vegetables", 
                "Dairy Products", 
                "Seafood" };

            var prodByCategory =
                from cat in categories
                join prod in this.clientContext.Products on cat equals prod.Category into ps
                from p in ps
                select new { Category = cat, p.ProductName };

            foreach (var item in prodByCategory)
            {
                Console.WriteLine(item.ProductName + ": " + item.Category);
            }
        }
        [TestCategory("Join Operators")]
        [TestMethod]//[Title("Left_Outer_Join_1")]
        [Description("A left outer join produces a result set that includes all the left hand side elements at " +
            "least once, even if they don't match any right hand side elements.")]
        public void Left_Outer_Join_1()
        {
            var supplierCusts =
                from sup in this.clientContext.Suppliers
                join cust in this.clientContext.Customers on sup.Country equals cust.Country into cs
                from c in cs.DefaultIfEmpty()  // DefaultIfEmpty preserves left-hand elements that have no matches on the right side 
                orderby sup.SupplierName
                select new
                {
                    Country = sup.Country,
                    CompanyName = c == null ? "(No customers)" : c.CompanyName,
                    SupplierName = sup.SupplierName
                };

            foreach (var item in supplierCusts)
            {
                Console.WriteLine("{0} ({1}): {2}", item.SupplierName, item.Country, item.CompanyName);
            }
        }

        [TestCategory("Join Operators")]
        [TestMethod]//[Title("Left_Outer_Join_2")]
        [Description("For each customer in the table of customers, this query returns all the suppliers " +
                     "from that same country, or else a string indicating that no suppliers from that country were found.")]
        public void Left_Outer_Join_2()
        {
            var custSuppliers =
                from cust in this.clientContext.Customers
                join sup in this.clientContext.Suppliers on cust.Country equals sup.Country into ss
                from s in ss.DefaultIfEmpty()
                orderby cust.CompanyName
                select new
                {
                    Country = cust.Country,
                    CompanyName = cust.CompanyName,
                    SupplierName = s == null ? "(No suppliers)" : s.SupplierName
                };

            foreach (var item in custSuppliers)
            {
                Console.WriteLine("{0} ({1}): {2}", item.CompanyName, item.Country, item.SupplierName);
            }
        }

        [TestCategory("Join Operators")]
        [TestMethod]//[Title("Left_Outer_Join_with_Composite_Key")]
        [Description("For each supplier in the table of suppliers, this query returns all the customers " +
                     "from the same city and country, or else a string indicating that no customers from that city/country were found. " +
                     "Note the use of anonymous types to encapsulate the multiple key values.")]
        public void Left_Outer_Join_with_Composite_Key()
        {
            var supplierCusts =
                from sup in this.clientContext.Suppliers
                join cust in this.clientContext.Customers on new { sup.City, sup.Country } equals new { cust.City, cust.Country } into cs
                from c in cs.DefaultIfEmpty() //Remove DefaultIfEmpty method call to make this an inner join
                orderby sup.SupplierName
                select new
                {
                    Country = sup.Country,
                    City = sup.City,
                    SupplierName = sup.SupplierName,
                    CompanyName = c == null ? "(No customers)" : c.CompanyName
                };

            foreach (var item in supplierCusts)
            {
                Console.WriteLine("{0} ({1}, {2}): {3}", item.SupplierName, item.City, item.Country, item.CompanyName);
            }
        }
    }
}
