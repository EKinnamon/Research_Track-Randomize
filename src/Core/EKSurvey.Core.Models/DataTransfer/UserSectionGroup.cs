using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using EKSurvey.Core.Extensions;
using EKSurvey.Core.Models.Enums;
using EKSurvey.Core.Models.Exceptions;

namespace EKSurvey.Core.Models.DataTransfer
{
    public class UserSectionGroup : IList<UserSection>, IUserSection
    {
        private readonly List<UserSection> _collection = new List<UserSection>();

        public UserSectionGroup() { }

        public UserSectionGroup(IEnumerable<UserSection> collection)
        {
            _collection = (collection ?? throw new ArgumentNullException(nameof(collection))).ToList();
            ThrowIfInvalid();
        }
        private void ThrowIfInvalid()
        {
            if (!_collection.Select(i => i.UserId).IsUnanimous())
                throw new InvalidSectionGroupConfigurationException("Sections are not owned by the same user id.");
            if (!_collection.Select(i => i.SurveyId).IsUnanimous())
                throw new InvalidSectionGroupConfigurationException("Sections are not associated to a single survey id.");
            if (!_collection.Select(i => i.TestId).IsUnanimous())
                throw new InvalidSectionGroupConfigurationException("Sections are not associated to a single test id.");
            if (!_collection.Select(i => i.Order).IsUnanimous())
                throw new InvalidSectionGroupConfigurationException("Sections must all belong to the same order.");
            if (!_collection.Select(i => i.Started).IsUnanimous())
                throw new InvalidSectionGroupConfigurationException("Sections must all have the same started date and time.");
            if (!_collection.Select(i => i.Modified).IsUnanimous())
                throw new InvalidSectionGroupConfigurationException("Sections must all have the same modified date and time.");
            if (!_collection.Select(i => i.Completed).IsUnanimous())
                throw new InvalidSectionGroupConfigurationException("Sections must all have the same modified date and time.");
        }

        public string UserId => _collection.FirstOrDefault()?.UserId;
        public int SurveyId => (_collection.FirstOrDefault()?.SurveyId).GetValueOrDefault();
        public Guid TestId => (_collection.FirstOrDefault()?.TestId).GetValueOrDefault();
        public int Id => (_collection.FirstOrDefault()?.Id).GetValueOrDefault();
        public int Order => (_collection.FirstOrDefault()?.Order).GetValueOrDefault();
        public DateTime? Started => _collection.FirstOrDefault()?.Started;
        public DateTime? Modified => _collection.FirstOrDefault()?.Modified;
        public DateTime? Completed => _collection.FirstOrDefault()?.Completed;
        public UserSection this[int index]
        {
            get => _collection[index];
            set => _collection[index] = value;
        }
        public int Count => _collection.Count;
        public bool IsReadOnly => false;

        public UserSection SelectedSection => _collection.SingleOrDefault(us => us.IsSelected.GetValueOrDefault(false));

        public SelectorType? SelectorType { get; set; }

        public IEnumerator<UserSection> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(UserSection item)
        {
            _collection.Add(item);
        }

        public void Clear()
        {
            _collection.Clear();
        }

        public bool Contains(UserSection item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(UserSection[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(UserSection item)
        {
            return _collection.Remove(item);
        }

        public int IndexOf(UserSection item)
        {
            return _collection.IndexOf(item);
        }

        public void Insert(int index, UserSection item)
        {
            _collection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _collection.RemoveAt(index);
        }

        public void AddRange(IEnumerable<UserSection> collection)
        {
            _collection.AddRange(collection);
        }
    }
}