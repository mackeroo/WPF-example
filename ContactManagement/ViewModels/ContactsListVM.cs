using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using ContactManagement.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Linq;
using System.Data.Entity.Validation;

namespace ContactManagement.ViewModels
{
    public class ContactsListVM : ViewModelBase
    {
        #region ContactSource enum
        public enum ContactSource { LIST, NEW, EDIT };
        #endregion ContactSource enum

        #region default constructor
        public ContactsListVM()
        {
            contacts = new ObservableCollection<Contact>();

            loadAllContactsCommand = new RelayCommand(LoadContacts, LoadContacts_canExexute);

        }
        #endregion default constructor

        #region Load all contacts

        //Binds to datagrid
        private ObservableCollection<Contact> contacts;

        public ObservableCollection<Contact> Contacts
        {
            get { return contacts; }
            set
            {
                if (contacts != null)
                {
                    contacts = value;
                    OnPropertyChanged("Contacts");
                }
            }
        }

        private ICommand loadAllContactsCommand;

        public ICommand LoadAllContactsCommand
        {
            get { return loadAllContactsCommand ?? (loadAllContactsCommand = new RelayCommand(() => LoadContacts())); }
        }



        public void LoadContacts()
        {


            // Get datacontext for contacts entities and assign it to local variable
            using (var context = new ContactsDatabaseEntities())
            {
                foreach (var contact in context.Contacts)
                {
                    contacts.Add(contact);
                }
            }

        }
        /// <summary>
        /// Checks how many contacts are loaded. If contacts are loaded (ie >0), then can execute
        /// </summary>
        public bool LoadContacts_canExexute()
        {
            if (contacts.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion Load all contacts

        #region Add new Contact
        private ICommand addContactCommand;

        public ICommand AddContactCommand
        {
            get { return addContactCommand ?? (addContactCommand = new RelayCommand(() => AddContact())); }
        }

        /// <summary>
        /// Creates a new contact (either Customer or Vendor) and adds it to the datagrid
        /// </summary>
        private void AddContact()
        {

            //Enable the add contact user control
            selectedContact = null;
            Messenger.Default.Send<NotificationMessage<Contact>>(new NotificationMessage<Contact>(selectedContact, "Message"), ContactSource.LIST);
            // receive contact to add from user control
            Messenger.Default.Register<NotificationMessage<Contact>>(this, ContactSource.NEW, ContactReceived);
        }

        /// <summary>
        /// Contact received from user control. If not null, add to the datacontext.
        /// Finally, untoggles button
        /// </summary>
        /// <param name="contact"></param>
        private void ContactReceived(NotificationMessage<Contact> contact)
        {
            if (contact.Content != null)
            {

                using (var context = new ContactsDatabaseEntities())
                {
                    if (context.VendorCompanies.Any(vc => vc.Vendor_Code == contact.Content.Vendor_Company_Code))
                    {
                        contact.Content.Vendor_Company_Code = null;
                    }
                    contacts.Add(contact.Content);
                    context.Contacts.Add(contact.Content);
                    context.SaveChanges();
                }
            }


            //untoggle button
            IsAddButtonPressed = false;

        }

        /// <summary>
        /// Class property to bind to Add New Contact Button
        /// </summary>
        private bool isAddButtonPressed;

        public bool IsAddButtonPressed
        {
            get { return isAddButtonPressed; }
            set
            {
                isAddButtonPressed = value;
                OnPropertyChanged("IsAddButtonPressed");
            }
        }


        #endregion //add new contact

        #region Edit selected Contact
        private Contact selectedContact;
        //bound to the first selected datagrid row
        public Contact SelectedContact
        {
            get { return selectedContact; }
            set
            {
                selectedContact = value;
                //calls the edit contact method when the datagrid selection changes
                EditContact();
                OnPropertyChanged("SelectedContact");
            }
        }

        /// <summary>
        /// Edits the selected contact on the datagrid
        /// </summary>
        private void EditContact()
        {
            if (selectedContact == null)
                return;

            //send contact to edit
            Messenger.Default.Send<NotificationMessage<Contact>>(new NotificationMessage<Contact>(selectedContact, "Message"),
                ContactSource.LIST);

            //receieve revised contact
            Messenger.Default.Register<NotificationMessage<Contact>>(this, ContactSource.EDIT, RevisedContact_Received);

        }

        /// <summary>
        /// Edited contact received by the user control
        /// </summary>
        /// <param name="contact"></param>
        private void RevisedContact_Received(NotificationMessage<Contact> contact)
        {
            if (contact.Content != null && selectedContact != null)
            {
                using (ContactsDatabaseEntities context = new ContactsDatabaseEntities())
                {
                    var vendorResult = context.VendorCompanies
                        .SingleOrDefault(vc => vc.Vendor_Code == contact.Content.Vendor_Company_Code);
                    if (vendorResult == null) // user changed the vendor company/code, create a new one
                    {
                        context.VendorCompanies.Add(contact.Content.VendorCompany);
                    }

                    //update entity
                    var contactResult = context.Contacts.SingleOrDefault(c => c.Id == selectedContact.Id);
                    if (contactResult != null)
                    {
                        contact.Content.Id = contactResult.Id;
                        context.Entry(contactResult).CurrentValues.SetValues(contact.Content);
                        context.SaveChanges();
                    }

                    //update observable collection
                    int index = contacts.IndexOf(contacts.Where(c => c == selectedContact).FirstOrDefault());
                    if (index > -1)
                    {
                        contacts[index] = contact.Content;

                    }
                }
            }
        }

        #endregion Edit selected conctact

    }

    #region INotifyPropertyChanged Implementation
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
    #endregion INotifyPropertyChanged Implementation
}
