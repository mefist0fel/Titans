using Model;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
    public sealed class UICancelTaskQueue : MonoBehaviour {
        [SerializeField]
        private UICancelTaskButton buttonPrefab; // Set from editor

        private struct TaskMarker {
            public Vector3 Position;
            public Titan.Task[] cancelTasks;
        }

        private readonly List<UICancelTaskButton> cancelButtons = new List<UICancelTaskButton>();
        private Titan titan;
        private List<TaskMarker> markers;

        public void Show(List<Titan.Task> tasks, Titan controlTitan) {
            titan = controlTitan;
            markers = SplitTaskMarkers(tasks);
            CreateNeedElementsCount(markers.Count);
            for (int i = 0; i < cancelButtons.Count; i++) {
                var needShow = i < markers.Count;
                if (needShow) {
                    cancelButtons[i].Show(markers[i].Position, i, CancelAction);
                } else {
                    cancelButtons[i].Hide();
                }
            }
        }

        private List<TaskMarker> SplitTaskMarkers(List<Titan.Task> tasks) {
            List<TaskMarker> markers = new List<TaskMarker>();
            for (int i = 0; i < tasks.Count; i++) {
                var task = tasks[i];
                var nextTask = i + 1 < tasks.Count ? tasks[i + 1]: null;
                if (task is MoveTask) {
                    var moveTask = task as MoveTask;
                    TaskMarker marker;
                    if (nextTask is ResourceTask) {
                        marker = new TaskMarker() { Position = moveTask.Position, cancelTasks = new Titan.Task[] { moveTask, nextTask as ResourceTask } };
                        i += 1;
                    } else {
                        marker = new TaskMarker() { Position = moveTask.Position, cancelTasks = new Titan.Task[] { moveTask} };
                    }
                    markers.Add(marker);
                }
                if (task is ResourceTask) {
                    var resourceTask = task as ResourceTask;
                    TaskMarker marker = new TaskMarker() { Position = resourceTask.Position, cancelTasks = new Titan.Task[] { resourceTask } };
                    markers.Add(marker);
                }
            }
            return markers;
        }

        private void CancelAction(int id) {
            if (titan == null)
                return;
            if (markers == null || id > markers.Count)
                return;
            titan.CancelTasks(markers[id].cancelTasks);
        }

        private void CreateNeedElementsCount(int count) {
            for (int i = cancelButtons.Count; i < count; i++) {
                cancelButtons.Add(CreateCancelButton());
            }
        }

        private UICancelTaskButton CreateCancelButton() {
            return Instantiate(buttonPrefab, transform);
        }

    }

}