using GalaSoft.MvvmLight.Command;
using ContactManagement.Models;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System;

namespace ContactManagement.ViewModels
{
    public class NewContactVM : ViewModelBase
    {
        #region variables
        public static int iMaxLengthTextbox = 3;
        #endregion variables

        #region default constructor
        public NewContactVM()
        {
            selectedContact = new Contact();
            acceptCommand = new RelayCommand(SendContact, SendContact_canExecute);
            cancelCommand = new RelayCommand(CancelSend, CancelSend_canExecute);
            CompanyNotExists = true;
            
            //receive selectedContact if available
            Messenger.Default.Register<NotificationMessage<Contact>>(this, ContactsListVM.ContactSource.LIST, SelectedContact_Received);
        }
        #endregion default constructor
        
        #region Check if new or existing contact
        private bool isNewContact;

        public bool IsNewContact
        {
            get { return isNewContact; }
            set
            {
                isNewContact = value;
                OnPropertyChanged("IsNewContact");
            }
        }

        /// <summary>
        /// receive selected contact and set to controls
        /// </summary>
        /// <param name="contact">selected contact from datagrid</param>
        private void SelectedContact_Received(NotificationMessage<Contact> contact)
        {
            
            IsViewEnabled = true;
            IsNewContact = contact.Content == null;
            if (!IsNewContact)
            {
                Name = contact.Content.Name.Trim();
                Type = contact.Content.Type.Trim();
                Address = contact.Content.Address.Trim();
                Company = contact.Content.Company.Trim();
                Phone_Number = contact.Content.Phone_Number;

                if (Type == "Vendor")
                {
                    VendorCodeKey = contact.Content.Vendor_Company_Code;
                    using (ContactsDatabaseEntities context = new ContactsDatabaseEntities())
                    {
                        CurrentVendorCompany = context.VendorCompanies.FirstOrDefault(vc => vc.Vendor_Code == contact.Content.Vendor_Company_Code);
                    }
                }
                else // Type = customer
                {
                    if (contact.Content.Notes != null)
                    {
                        Notes = contact.Content.Notes.Trim();
                    }
                    else
                    {
                        Notes = null;
                    }
                }
            }
            else //new contact
            {
                ClearForm();
            }
            
        }

        #endregion Check if new or existing contact

        #region View visibility
       
        //Bind to Visibility using a boolToVisConverter
        private bool isViewEnabled;

        public bool IsViewEnabled
        {
            get { return isViewEnabled; }
            set
            {
                isViewEnabled = value;
                OnPropertyChanged("IsViewEnabled");
            }
        }
        #endregion View visibility

        #region Contact class and properties
        private Contact selectedContact;

        public Contact ContactToAdd
        {
            get { return selectedContact; }
            set
            {
                selectedContact = value;
                
                OnPropertyChanged("ContactToAdd");
            }
        }

        public string Type
        {
            get { return selectedContact.Type; }
            set
            {
                selectedContact.Type = value;
                AcceptCreateCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("Type");
            }
        }

        public string Name
        {
            get { return selectedContact.Name; }
            set
            {
                selectedContact.Name = value;
                AcceptCreateCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("Name");
            }
        }

        public string Company
        {
            get { return selectedContact.Company; }
            set
            {
                selectedContact.Company = value;

                // Only for vendors, search user-entered vendor company in the vendor company master list.
                // If vendor company exists, load vendor code (read-only)
                // If vendor company doesn't exist in master list, create a new one w/ manual vendor code.
                if (selectedContact.Type == "Vendor" && selectedContact.Company.Length > iMaxLengthTextbox)
                {
                    using (ContactsDatabaseEntities context = new ContactsDatabaseEntities())
                    {
                        // vendor company value matches vendor company in master list
                        selectedContact.VendorCompany = context.VendorCompanies.FirstOrDefault(vc => vc.Name == selectedContact.Company);
                        if (selectedContact.VendorCompany != null)
                        {
                            CompanyNotExists = false;
                            VendorCodeKey = selectedContact.VendorCompany.Vendor_Code;

                        }
                        else
                        {
                            //company vendor code is now enabled
                            CompanyNotExists = true;

                            //vendor doesn't exist in master, if vendor key text exists in master, then set to null
                            if (context.VendorCompanies.Any(vc => vc.Vendor_Code == VendorCodeKey))
                            {
                                VendorCodeKey = null;

                            }
                        }
                    }
                }
                AcceptCreateCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("Company");
            }
        }


        // Bind to vendor code isenabled
        private bool companyNotExists;

        public bool CompanyNotExists
        {
            get { return companyNotExists; }
            set
            {
                companyNotExists = value;
                OnPropertyChanged("CompanyExists");
            }
        }


        public string Address
        {
            get { return selectedContact.Address; }
            set
            {
                selectedContact.Address = value;
                AcceptCreateCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("Address");
            }
        }

        public long? Phone_Number
        {
            get { return selectedContact.Phone_Number; }
            set
            {
                selectedContact.Phone_Number = value;
                AcceptCreateCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("Phone_Number");
            }
        }

        public string Notes
        {
            get { return selectedContact.Notes; }
            set
            {
                selectedContact.Notes = value;
                OnPropertyChanged("Notes");
            }
        }

        public string VendorCodeKey
        {
            get { return selectedContact.Vendor_Company_Code; }
            set
            {
                selectedContact.Vendor_Company_Code = value;
                AcceptCreateCommand.RaiseCanExecuteChanged();
                OnPropertyChanged("CompanyNotExists");
                OnPropertyChanged("VendorCodeKey");
            }
        }

        //vendor company

        public VendorCompany CurrentVendorCompany
        {
            get { return selectedContact.VendorCompany; }
            set
            {
                selectedContact.VendorCompany = value;
                OnPropertyChanged("CurrentVendorCompany");
            }
        }

        public string VendorCompanyName
        {
            get { return selectedContact.VendorCompany.Name; }
            set
            {
                selectedContact.VendorCompany.Name = value;
                OnPropertyChanged("VendorCompanyName");
            }
        }
        #endregion Contact class and properties

        #region Accept Button Clicked - Send Message
        private RelayCommand acceptCommand;

        public RelayCommand AcceptCreateCommand
        {
            get { return acceptCommand; }
            set { acceptCommand = value ?? (new RelayCommand(() => SendContact())); }
        }

        /// <summary>
        /// Updates the Contacts database with a new contact
        /// </summary>
        private void SendContact()
        {
            // if Vendor company does not exist in the master list, add to master
            if (Type == "Vendor")
            {
                if (selectedContact.VendorCompany == null)
                {
                    selectedContact.VendorCompany = new VendorCompany
                    {
                        Name = selectedContact.Company,
                        Vendor_Code = selectedContact.Vendor_Company_Code
                    };
                }
                else // set Vendor company to null so that Entity Framework does not try to add a duplicate vendor in master list
                {
                    selectedContact.VendorCompany = null;
                }
            }
            

            // Set message token based on whether a contact was receieved by the user control
            object token;
            if (isNewContact)
            {
                token = ContactsListVM.ContactSource.NEW;
            }
            else
            {
                token = ContactsListVM.ContactSource.EDIT;
            }
            Messenger.Default.Send<NotificationMessage<Contact>>(new NotificationMessage<Contact>(selectedContact, "Message"), token);
            IsViewEnabled = false;

            ClearForm(); // clear all fields

        }
        /// <summary>
        /// Func<bool> for accept command can execute. Checks if all proper fields are filled
        /// before enabling accept button
        /// </summary>
        private bool SendContact_canExecute()
        {
            //all necessary fields are filled
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Company) && !string.IsNullOrEmpty(Address) &&
               !string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(Phone_Number.ToString()))
            {
                if (Type == "Vendor")
                {
                    return (!string.IsNullOrEmpty(VendorCodeKey));
                }
                else
                {
                    return true;
                }

            }
            else
            {
                return false;
            }
        }
        #endregion Accept Button Clicked - Send Message

        #region Cancel Button Clicked

        private RelayCommand cancelCommand;

        public RelayCommand CancelCreateCommand
        {
            get { return cancelCommand; }
            set { cancelCommand = value ?? (new RelayCommand(() => CancelSend())); }
        }
        /// <summary>
        /// set view to visibility.collapsed
        /// </summary>
        private void CancelSend()
        {
            IsViewEnabled = false;
            //send null contact back to datagrid user control
            selectedContact = null;
            Messenger.Default.Send<NotificationMessage<Contact>>(new NotificationMessage<Contact>(selectedContact, "Message"));
            ClearForm(); // clear all fields in the form
        }
        /// <summary>
        /// Always returns true
        /// </summary>
        private bool CancelSend_canExecute()
        {
            return true;
        }

        #endregion Cancel Button Clicked

        #region On Control Close
        private void ClearForm()
        {
            selectedContact = new Contact();
            Name = null;
            Type = null;
            Company = null;
            Address = null;
            Phone_Number = null;
            Notes = null;
            VendorCodeKey = null;
        }
        #endregion
    }

}
