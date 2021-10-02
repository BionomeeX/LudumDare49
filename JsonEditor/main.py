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
        self.updateJsonButton.clicked.connect(self.updateJson)

        self.addDialogButton.clicked.connect(lambda: self.addNewConversation(self.typeOfSentence.currentText()))

        self.show()

    def select_file(self):
        # fname = QFileDialog.getOpenFileName(self, 'Open file', '', 'Images (*.json)')
        fname = QFileDialog.getOpenFileName(self, 'Open file', '')
        self.selectedFilePath.setText(fname[0])
        self.load_json(fname[0])

    def load_json(self, path):
        with open(path, "r") as f:
            try:
                self.leader_data = json.load(f)[0]
            except JSONDecodeError:
                self.leader_data = dict()
        print(self.leader_data)

        try:
            self.textTrigram.setPlainText(self.leader_data["trigram"])
            self.textName.setPlainText(self.leader_data["leaderName"])
            self.textDomainName.setPlainText(self.leader_data["domainName"])
            self.textMaxSanity.setPlainText(str(self.leader_data["maxSanity"]))

            self.sentencesConversation.setPlainText(str(self.leader_data["sentencesConversation"]))

            self.sentencesCrisis.setPlainText(str(self.leader_data["sentencesCrisis"]))
            self.sentencesEvent.setPlainText(str(self.leader_data["sentencesEvent"]))
        except Exception as e:
            print(e)

    def updateJson(self):
        try:
            self.leader_data["trigram"] = self.textTrigram.toPlainText()
            self.leader_data["leader_name"] = self.textName.toPlainText()
            self.leader_data["domainName"] = self.textMaxSanity.toPlainText()
            self.leader_data["maxSanity"] = self.textMaxSanity.toPlainText()

            with open(self.selectedFilePath.text(), "w") as f:
                json.dump(self.leader_data, f)
        except Exception as e:
            print(e)

    def addNewConversation(self, typeOfSentence):
        try:
            if self.textDialog.toPlainText() != "" and \
                    0 <= int(self.textDangerLevel.toPlainText()) <= int(self.textMaxSanity.toPlainText()):
                if not self.textDangerLevel.toPlainText() in self.leader_data[typeOfSentence]:
                    self.leader_data[typeOfSentence][self.textDangerLevel.toPlainText()] = []
                    self.leader_data[typeOfSentence][self.textDangerLevel.toPlainText()].append(self.textDialog.toPlainText())
                else:
                    self.leader_data[typeOfSentence][self.textDangerLevel.toPlainText()].append(
                        self.textDialog.toPlainText())
                self.sentencesConversation.setPlainText(str(self.leader_data[typeOfSentence]))
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
