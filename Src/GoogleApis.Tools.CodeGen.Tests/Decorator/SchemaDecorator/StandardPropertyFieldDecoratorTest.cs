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
using System.CodeDom;
using System.Collections.Generic;

using Newtonsoft.Json.Schema;
using NUnit.Framework;

using Google.Apis.Discovery.Schema;
using Google.Apis.Tools.CodeGen.Decorator.SchemaDecorator;
using Google.Apis.Tools.CodeGen.Tests.Generator;

namespace Google.Apis.Tools.CodeGen.Tests.Decorator.SchemaDecorator
{
    [TestFixture()]
    public class StandardPropertyFieldDecoratorTest
    {
        internal static readonly IDictionary<string, JsonSchemaType> NamesToType = new SortedDictionary<string, JsonSchemaType>(){
            {"a", JsonSchemaType.Boolean},
            {"b", JsonSchemaType.Integer},
            {"c", JsonSchemaType.Float},
            {"d", JsonSchemaType.String},
            {"e", JsonSchemaType.Boolean},
            {"f", JsonSchemaType.Integer},
        };
        
        private IInternalClassProvider internalClassProvider = new ObjectInternalClassProvider();
        
        [Test()]
        public void GenerateAllFieldsTestEdgeCases()
        {
            var decorator = new StandardPropertyFieldDecorator();
            var schema = new MockSchema();
            Assert.Throws(typeof(ArgumentNullException), () => decorator.GenerateAllFields(null, internalClassProvider));
            Assert.Throws(typeof(ArgumentNullException), () => decorator.GenerateAllFields(schema, null));
            
            schema.Name = "test";
            schema.SchemaDetails = null;
            Assert.Throws(typeof(ArgumentNullException), () => decorator.GenerateAllFields(schema, internalClassProvider));
            
            schema.SchemaDetails = new JsonSchema();
            schema.SchemaDetails.Properties = null;
            var expectedEmpty = decorator.GenerateAllFields(schema, internalClassProvider);
            Assert.IsNotNull(expectedEmpty);
            Assert.AreEqual(0, expectedEmpty.Count);
        }
        
        [Test()]
        public void GenerateAllFieldsTest ()
        {
            var schema = new MockSchema();
            schema.SchemaDetails = new JsonSchema();
            schema.SchemaDetails.Type = JsonSchemaType.Object;
            schema.SchemaDetails.Properties = new Dictionary<string, JsonSchema>();
            foreach(var pair in NamesToType)
            {
                JsonSchema property = new JsonSchema();
                property.Type = pair.Value;
                property.Id = pair.Key;
                schema.SchemaDetails.Properties.Add(pair.Key, property);
            }
            
            var decorator = new StandardPropertyFieldDecorator();
            IList<CodeMemberField> generatedFields = decorator.GenerateAllFields(schema, internalClassProvider);
            
            Assert.NotNull(generatedFields);
            Assert.AreEqual(NamesToType.Count, generatedFields.Count);
            foreach(var field in generatedFields)
            {
                Assert.That(NamesToType.ContainsKey(field.Name), "field name was not present in namesToType " + field.Name);                
            }
            
            int item = 0;
            foreach(var pair in NamesToType)
            {
                var field = generatedFields[item++];
                Assert.AreEqual(pair.Key, field.Name, string.Format("Name different for expected at index {0}", item -1));
                Assert.AreEqual(SchemaDecoratorUtil.GetCodeType(new JsonSchema(){Type = pair.Value}, internalClassProvider).BaseType, 
                                field.Type.BaseType);
            }
        }
        
        [Test()]
        public void GenerateFieldTest ()
        {
            var schema = new JsonSchema();
            var decorator = new StandardPropertyFieldDecorator();
            
            schema.Type = JsonSchemaType.String;
            CodeMemberField generatedField = decorator.GenerateField("normalName", schema, 1, internalClassProvider);
            
            Assert.NotNull(generatedField);
            Assert.AreEqual(typeof(string).FullName, generatedField.Type.BaseType);
            Assert.AreEqual("normalName", generatedField.Name);
            Assert.AreEqual(MemberAttributes.Private, generatedField.Attributes);
            
            schema.Type = JsonSchemaType.Boolean;
            generatedField = decorator.GenerateField("public", schema, 2, internalClassProvider);
            
            Assert.NotNull(generatedField);
            Assert.AreEqual(typeof(bool).FullName, generatedField.Type.BaseType);
            Assert.AreEqual("publicField2", generatedField.Name);
            Assert.AreEqual(MemberAttributes.Private, generatedField.Attributes);
            
            generatedField = decorator.GenerateField("UPPERCASE", schema, 2, internalClassProvider);
            
            Assert.NotNull(generatedField);
            Assert.AreEqual("uPPERCASE", generatedField.Name);
            
        }
    }
}

