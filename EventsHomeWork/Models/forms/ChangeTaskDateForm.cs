using EventsHomeWork.Abstrctions;
using EventsHomeWork.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using static EventsHomeWork.Models.ChangeTaskDateForm;

namespace EventsHomeWork.Models
{
    public class ChangeTaskDateForm(int formTaskId) : IUserForm
    {
        public enum Step
        {
            WaitingForDate,
            WaitingForReason,
            Completed
        }
        public Step CurrentStep { get; private set; } = Step.WaitingForDate;
        public DateTime? SelectedDate { get; private set; }
        public string? Reason { get; private set; }
        private readonly int _formTaskId = formTaskId;
        public bool IsCompleted => CheckIsFormCompleted();
        public string? AnswerMessage { get; set; }

        public string GetFormData(string userMessage)
        {
            return CurrentStep switch
            {
                Step.WaitingForDate => FormOnStepDate(userMessage),
                Step.WaitingForReason => FormOnStepReason(userMessage),
                Step.Completed => FormOnStepCompleted(userMessage),
                _ => FormOnStepNotFound()
            };
        }

        public string GetFormRequestBody(int currentUserId)
        {
            var _requestBody = new
            {
                answerType = AnswerTypes.ChangeTaskDate.ToString(),
                formTaskId = _formTaskId,
                formChangerId = currentUserId,
                formNewDate = SelectedDate,
                formNewReason = Reason
            };
            string _json = JsonConvert.SerializeObject(_requestBody);

            return _json;
        }

        private string FormOnStepDate(string dateStepMessage)
        {

            if (DateTime.TryParse(dateStepMessage, out DateTime _date))
            {
                if (_date <= DateTime.Now)
                    return "Новый срок должен быть больше текщуей даты";
                
                SelectedDate = _date;
                CurrentStep = Step.WaitingForReason;
                return "Укажите причину";
            }
            else
                return "Укажатие дату в формате:\r\n\"ГГГГ-ММ-ДД ЧЧ:ММ\"";
        }

        private string FormOnStepReason(string dateStepMessage)
        {
            Reason = dateStepMessage;
            CurrentStep = Step.Completed;
            return "Форма успешно заполнена";
        }

        private string FormOnStepCompleted(string reasonStepMessage)
        {
            Reason = reasonStepMessage;
            return "Форма успешно заполнена и отправлена";
        }

        private static string FormOnStepNotFound()
        {
            throw new Exception("Шаг не распознан");
        }

        private bool CheckIsFormCompleted()
        {
            if (CurrentStep == Step.Completed && SelectedDate is not null && Reason is not null)
                return true;
            else
                return false;
        }
    }
}
