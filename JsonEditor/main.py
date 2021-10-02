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
        self.addEffectButton.clicked.connect(self.add_effect)

        self.radioSentenceMod.clicked.connect(self.show_sentence_mod)
        self.radioCardMod.clicked.connect(self.show_card_mod)
        self.reduceButton.setEnabled(False)
        self.increaseButton.setEnabled(False)
        self.show_sentence_mod()
        self.show()

    def add_effect(self):
        try:
            if self.textEffectKey.toPlainText() == "" or self.textEffectValue.toPlainText() == "":
                self.statusMessage.setText("ERROR: effectKey and effectValue must be filled")
            else:
                self.effects[self.textEffectKey.toPlainText()] = self.textEffectValue.toPlainText()
                self.Effects.setPlainText(str(self.effects))
        except Exception as e:
            print(e)

    def reset_effects(self):
        self.effects = dict()
        self.Effects.setPlainText("")

    def show_card_mod(self):
        self.reset_effects()
        self.sentenceMod.setVisible(False)
        self.cardMod.setVisible(True)

    def show_sentence_mod(self):
        self.sentenceMod.setVisible(True)
        self.cardMod.setVisible(False)

    def reduce_leader_idx(self):
        self.reset_effects()
        self.statusMessage.setText("")
        if self.leader_idx > 0:
            self.leader_idx -= 1
            self.leaderIdx.setPlainText(str(self.leader_idx))
            self.switch_leader()

    def increase_leader_idx(self):
        self.reset_effects()
        self.statusMessage.setText("")
        self.leader_idx += 1
        self.leaderIdx.setPlainText(str(self.leader_idx))
        print(len(self.leader_data))
        if self.leader_idx <= len(self.leader_data) - 1:
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
                self.trigram.setCurrentText(self.leader_data[self.leader_idx]["trigram"])
                self.textName.setPlainText(self.leader_data[self.leader_idx]["leaderName"])
                self.textDomainName.setPlainText(self.leader_data[self.leader_idx]["domainName"])
                self.textMaxSanity.setPlainText(str(self.leader_data[self.leader_idx]["maxSanity"]))

                self.sentencesConversation.setPlainText(str(self.leader_data[self.leader_idx]["sentencesConversation"]))

                self.sentencesCrisis.setPlainText(str(self.leader_data[self.leader_idx]["sentencesCrisis"]))
                self.sentencesEvent.setPlainText(str(self.leader_data[self.leader_idx]["sentencesEvent"]))

                self.cards.setPlainText(str(self.leader_data[self.leader_idx]["cards"]))
                self.reduceButton.setEnabled(True)
                self.increaseButton.setEnabled(True)
            except Exception as e:
                print(e)
        else:
            self.leader_data.append(dict())
            self.leader_data[self.leader_idx]["sentencesConversation"] = dict()
            self.leader_data[self.leader_idx]["sentencesCrisis"] = dict()
            self.leader_data[self.leader_idx]["sentencesEvent"] = dict()
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
            self.leader_data[self.leader_idx]["trigram"] = self.trigram.currentText()
            self.leader_data[self.leader_idx]["leaderName"] = self.textName.toPlainText()
            self.leader_data[self.leader_idx]["domainName"] = self.textDomainName.toPlainText()
            self.leader_data[self.leader_idx]["maxSanity"] = self.textMaxSanity.toPlainText()


            with open(self.selectedFilePath.text(), "w") as f:
                json.dump(self.leader_data, f)
            self.statusMessage.setText("SUCCESS:")
        except Exception as e:
            self.statusMessage.setText("ERROR:")
            print(e)

    def add_new_conversation(self, typeOfSentence):
        try:
            print(self.textDangerLevel.toPlainText())
            print(self.textMaxSanity.toPlainText())
            if self.textDialog.toPlainText() == "" or self.textMaxSanity.toPlainText() == "":
                self.statusMessage.setText("ERROR: some fields are empty")
            elif int(self.textDangerLevel.toPlainText()) < 0 or \
                    int(self.textDangerLevel.toPlainText()) > int(self.textMaxSanity.toPlainText()):
                self.statusMessage.setText("ERROR: danger level must be lower than max sanity")
            else:
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
                self.statusMessage.setText("SUCCESS:")
        except Exception as e:
            print(e)


if __name__ == '__main__':
    os.environ["QT_AUTO_SCREEN_SCALE_FACTOR"] = "1"
    app = QtWidgets.QApplication(sys.argv)
    window = UI()
    app.exec_()
