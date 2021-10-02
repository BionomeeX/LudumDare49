import os
import sys
from json import JSONDecodeError

from PyQt5 import QtWidgets, uic
from PyQt5.QtGui import QPixmap
from PyQt5.QtCore import QObject, QThread, pyqtSignal, QFile

from PyQt5.QtWidgets import QLabel, QComboBox, QPushButton, QFileDialog
import json


class UI(QtWidgets.QMainWindow):
    def __init__(self):
        super(UI, self).__init__()
        uic.loadUi('qt_ui/main.ui', self)

        self.selectFileButton.clicked.connect(self.select_file)
        self.updateJsonButton.clicked.connect(self.update_json)
        self.reduceButton.clicked.connect(self.reduce_leader_idx)
        self.increaseButton.clicked.connect(self.increase_leader_idx)
        self.addDialogButton.clicked.connect(lambda: self.add_new_conversation(self.typeOfSentence.currentText()))

        self.reduceButton.setEnabled(False)
        self.increaseButton.setEnabled(False)
        self.show()

    def reduce_leader_idx(self):
        if self.leader_idx > 0:
            self.leader_idx -= 1
            self.leaderIdx.setPlainText(str(self.leader_idx))
            self.switch_leader()


    def increase_leader_idx(self):
        self.leader_idx += 1
        self.leaderIdx.setPlainText(str(self.leader_idx))
        if self.leader_idx < len(self.leader_data) - 1:
            self.switch_leader(False)
        else:
            self.switch_leader(True)

    def select_file(self):
        # fname = QFileDialog.getOpenFileName(self, 'Open file', '', 'Images (*.json)')
        fname = QFileDialog.getOpenFileName(self, 'Open file', '')
        self.selectedFilePath.setText(fname[0])
        self.load_json(fname[0])

    def switch_leader(self, is_new=False):
        if is_new == False:
            try:
                self.textTrigram.setPlainText(self.leader_data[self.leader_idx]["trigram"])
                self.textName.setPlainText(self.leader_data[self.leader_idx]["leaderName"])
                self.textDomainName.setPlainText(self.leader_data[self.leader_idx]["domainName"])
                self.textMaxSanity.setPlainText(str(self.leader_data[self.leader_idx]["maxSanity"]))

                self.sentencesConversation.setPlainText(str(self.leader_data[self.leader_idx]["sentencesConversation"]))

                self.sentencesCrisis.setPlainText(str(self.leader_data[self.leader_idx]["sentencesCrisis"]))
                self.sentencesEvent.setPlainText(str(self.leader_data[self.leader_idx]["sentencesEvent"]))
                self.reduceButton.setEnabled(True)
                self.increaseButton.setEnabled(True)
            except Exception as e:
                print(e)
        else:
            self.textTrigram.setPlainText("")
            self.textName.setPlainText("")
            self.textDomainName.setPlainText("")
            self.textMaxSanity.setPlainText("")

            self.sentencesConversation.setPlainText("")

            self.sentencesCrisis.setPlainText("")
            self.sentencesEvent.setPlainText("")
            self.reduceButton.setEnabled(True)
            self.increaseButton.setEnabled(True)


    def load_json(self, path):
        self.leader_idx = 0
        with open(path, "r") as f:
            try:
                self.leader_data = json.load(f)
            except JSONDecodeError:
                self.leader_data = list()
        self.switch_leader()

    def update_json(self):
        try:
            self.leader_data[self.leader_idx]["trigram"] = self.textTrigram.toPlainText()
            self.leader_data[self.leader_idx]["leader_name"] = self.textName.toPlainText()
            self.leader_data[self.leader_idx]["domainName"] = self.textMaxSanity.toPlainText()
            self.leader_data[self.leader_idx]["maxSanity"] = self.textMaxSanity.toPlainText()
            with open(self.selectedFilePath.text(), "w") as f:
                json.dump(self.leader_data, f)
        except Exception as e:
            print(e)

    def add_new_conversation(self, typeOfSentence):
        try:
            if self.textDialog.toPlainText() != "" and \
                    0 <= int(self.textDangerLevel.toPlainText()) <= int(self.textMaxSanity.toPlainText()):
                if not self.textDangerLevel.toPlainText() in self.leader_data[self.leader_idx][typeOfSentence]:
                    self.leader_data[self.leader_idx][typeOfSentence][self.textDangerLevel.toPlainText()] = []
                    self.leader_data[self.leader_idx][typeOfSentence][self.textDangerLevel.toPlainText()].append(
                        self.textDialog.toPlainText())
                else:
                    self.leader_data[self.leader_idx][typeOfSentence][self.textDangerLevel.toPlainText()].append(
                        self.textDialog.toPlainText())
                self.sentencesConversation.setPlainText(str(self.leader_data[self.leader_idx][typeOfSentence]))
                self.textDialog.setPlainText("")
                self.textDangerLevel.setPlainText("")
                self.statusMessage.setPlainText("SUCCESS")
            else:
                self.statusMessage.setPlainText("ERROR some fields are empty")


        except Exception as e:
            print(e)


if __name__ == '__main__':
    os.environ["QT_AUTO_SCREEN_SCALE_FACTOR"] = "1"
    app = QtWidgets.QApplication(sys.argv)
    window = UI()
    app.exec_()
