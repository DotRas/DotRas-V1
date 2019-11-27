//--------------------------------------------------------------------------
// <copyright file="RasEntryTest.cs" company="Jeff Winn">
//      Copyright (c) Jeff Winn. All rights reserved.
//
//      The use and distribution terms for this software is covered by the
//      GNU Library General Public License (LGPL) v2.1 which can be found
//      in the License.rtf at the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by
//      the terms of this license.
//
//      You must not remove this notice, or any other, from this software.
// </copyright>
//--------------------------------------------------------------------------

namespace DotRas.IntegrationTests
{
    using System;
    using System.Linq;
    using System.Net;
    using DotRas.Internal;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for <see cref="DotRas.RasEntry"/> and is intended to contain all associated integration tests.
    /// </summary>
    [TestClass]
    public class RasEntryTest : IntegrationTest
    {
        #region Fields

        private string _entryName;
        private string _phoneBookPath;
        private RasPhoneBook _phoneBook;
        private RasEntry _entry;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RasEntryTest"/> class.
        /// </summary>
        public RasEntryTest()
        {
        }

        #endregion

        #region Methods

        #region ~ Test Methods Init

        /// <summary>
        /// Initializes the test instance.
        /// </summary>
        [TestInitialize]
        [TestCategory("Integration")]
        public override void Initialize()
        {
            base.Initialize();

            this._phoneBookPath = System.IO.Path.GetTempFileName();
            this._entryName = Guid.NewGuid().ToString();

            this._phoneBook = RasPhoneBook.Open(this._phoneBookPath);

            this._entry = new RasEntry(this._entryName);
            this._entry.Device = RasDevice.GetDevices().Where(o => o.Name.Contains("(PPTP)") && o.DeviceType == RasDeviceType.Vpn).FirstOrDefault();
            this._entry.EncryptionType = RasEncryptionType.Require;
            this._entry.EntryType = RasEntryType.Vpn;
            this._entry.FramingProtocol = RasFramingProtocol.Ppp;
            this._entry.NetworkProtocols.IP = true;
            this._entry.NetworkProtocols.IPv6 = true;
            this._entry.PhoneNumber = IPAddress.Loopback.ToString();
            this._entry.VpnStrategy = RasVpnStrategy.Default;

            this._phoneBook.Entries.Add(this._entry);
        }

        /// <summary>
        /// Performs cleanup for the current test instance.
        /// </summary>
        [TestCleanup]
        public void CleanUp()
        {
            if (this._phoneBook.Entries.Contains(this._entry))
            {
                this._phoneBook.Entries.Remove(this._entry);
            }
            
            System.IO.File.Delete(this._phoneBookPath);
        }

        #endregion

        #region ClearCredentials

        /// <summary>
        /// Tests the ClearCredentials method to ensure the credentials will be cleared as expected.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void ClearCredentialsTest()
        {
            bool expected = true;
            bool actual = this._entry.ClearCredentials();

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region GetCredentials

        /// <summary>
        /// Tests the GetCredentials method to ensure the expected credentials are returned.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void GetCredentialsTest()
        {
            NetworkCredential expected = new NetworkCredential("User", "Password", "Domain");

            // Update the credentials so they can be retrieved correctly.
            this._entry.UpdateCredentials(expected);

            var actual = this._entry.GetCredentials();

            Assert.AreEqual(expected.UserName, actual.UserName);
            Assert.AreEqual(expected.Domain, actual.Domain);
        }

        #endregion

        #region UpdateCredentials

        /// <summary>
        /// Tests the UpdateCredentials method to ensure the credentials will update.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public void UpdateCredentialsTest()
        {
            NetworkCredential credentials = new NetworkCredential("Test", "User", "Domain");
            bool expected = true;

            bool actual = this._entry.UpdateCredentials(credentials);

            Assert.AreEqual(expected, actual);
        }

#if (WINXP || WIN2K8 || WIN7 || WIN8)

        /// <summary>
        /// Tests the UpdateCredentials method to ensure the credentials will update for the current user.
        /// </summary>
        [TestMethod]
        public void UpdateCredentialsForUserTest()
        {
            NetworkCredential credentials = new NetworkCredential("Test", "User", "Domain");
            bool expected = true;

            bool actual = this._entry.UpdateCredentials(credentials, false);

            Assert.AreEqual(expected, actual);
        }

#endif

        #endregion

        #endregion
    }
}