import os
import sys
from json import JSONDecodeError

from PyQt5 import QtWidgets, uic
from PyQt5.QtGui import QPixmap
from PyQt5.QtCore import QObject, QThread, pyqtSignal, QFile

from PyQt5.QtWidgets import QLabel, QComboBox, QPushButton, QFileDialog
import json
import pprint

class EditorSelector(QtWidgets.QMainWindow):
    def __init__(self):
        super(EditorSelector, self).__init__()
        uic.loadUi('qt_ui/editorSelector.ui', self)

        self.openEditorButton.clicked.connect(self.openEditor)
        self.show()

    def openEditor(self):
        try:

            if self.leaderRadioButton.isChecked():
                self.window = LeaderEditor()
            else:
                self.window = EventEditor()
            window.show()
            self.hide()
        except Exception as e:
            print(e)


class EventEditor(QtWidgets.QMainWindow):
    def __init__(self):
        try:
            super(EventEditor, self).__init__()
            uic.loadUi('qt_ui/eventEditor.ui', self)
            self.event_idx = 0
            self.selectFileButton.clicked.connect(self.select_file)

            self.addChoiceButton.clicked.connect(self.add_choice)

            self.reduceButton.clicked.connect(self.reduce_event_idx)
            self.increaseButton.clicked.connect(self.increase_event_idx)

            self.addRequirementButton.clicked.connect(self.add_requirement)
            self.resetRequirementButton.clicked.connect(self.reset_requirements)
            self.updateJsonButton.clicked.connect(self.update_json)
            self.endIdxButton.clicked.connect(self.go_to_end_idx)

            self.addEffectButton.clicked.connect(self.add_effects)
            self.resetEffectButton.clicked.connect(self.reset_effects)
            self.reset_requirements()
            self.reset_effects()

            self.show()
        except Exception as e:
            print(e)


    def select_file(self):
        # fname = QFileDialog.getOpenFileName(self, 'Open file', '', 'Images (*.json)')
        fname = QFileDialog.getOpenFileName(self, 'Open file', '')
        self.selectedFilePath.setText(fname[0])
        if fname[0].endswith('.json'):
            self.load_json(fname[0])
            self.statusMessage.setText("")
        else:
            self.statusMessage.setText("ERROR: Loaded file must .json")

    def load_json(self, path):
        print("OK")
        self.restore_empty_UI()
        self.event_idx = 0
        with open(path, "r") as f:
            try:
                self.event_data = json.load(f)
                self.switch_event(False)
            except JSONDecodeError as e:
                self.event_data = list()
                self.switch_event(True)

    def restore_empty_UI(self):
        self.textName.setPlainText("")
        self.textDescription.setPlainText("")

        self.choices.setPlainText("")

        self.textChoiceDescription.setPlainText("")
        self.textChoiceCost.setPlainText("")

        self.existingRequirements.setPlainText("")
        self.textRequirementValue.setPlainText("")

    def switch_event(self, is_new=False):
        print(is_new)
        try:
            if is_new == False:
                print(self.event_idx)
                self.textName.setPlainText(self.event_data[self.event_idx]["name"])
                self.textDescription.setPlainText(self.event_data[self.event_idx]["description"])

                self.isCrisisCheckbox.setCheckState(self.event_data[self.event_idx]["isCrisis"])
                self.choices.setPlainText(str(self.event_data[self.event_idx]["choices"]))
                self.reduceButton.setEnabled(True)
                self.increaseButton.setEnabled(True)
            else:
                self.init_base_structure()
                self.restore_empty_UI()
                self.reduceButton.setEnabled(True)
                self.increaseButton.setEnabled(True)
        except Exception as e:
            print(e)

    def add_choice(self):
        try:
            if self.textChoiceDescription.toPlainText() == "" or self.textChoiceCost.toPlainText() == "":
                self.statusMessage.setText("ERROR: name, description and cost must be filled")
            else:
                tmpdict = dict()
                tmpdict["description"] = self.textChoiceDescription.toPlainText()
                tmpdict["targetTrigram"] = self.choiceTrigram.currentText()
                tmpdict["cost"] = self.textChoiceCost.toPlainText()
                tmpdict["requirements"] = self.requirements
                tmpdict["effects"] = self.effects
                self.event_data[self.event_idx]["choices"].append(tmpdict)

                self.choices.setPlainText(str(self.event_data[self.event_idx]["choices"]))
        except Exception as e:
            print(e)



    def add_requirement(self):
        try:
            if self.textRequirementValue.toPlainText() == "":
                self.statusMessage.setText("ERROR: requirement must be filled must be filled")
            else:
                self.requirements[self.requirementKeyComboBox.currentText()] = self.textRequirementValue.toPlainText()
                self.existingRequirements.setPlainText(str(self.requirements))
        except Exception as e:
            print(e)

    def reset_requirements(self):
        self.requirements = dict()
        self.existingRequirements.setPlainText("")

    def add_effects(self):
        try:
            if self.textMethodName.toPlainText() == "":
                self.statusMessage.setText("ERROR: textMethodName must be filled must be filled")
            else:
                tmpdict = dict()
                tmpdict["methodName"] = self.textMethodName.toPlainText()
                tmpdict["argument"] = self.textArgument.toPlainText()
                self.effects.append(tmpdict)
                self.existingEffects.setPlainText(str(self.effects))
        except Exception as e:
            print(e)

    def reset_effects(self):
        self.effects = list()
        self.existingEffects.setPlainText("")

    def go_to_end_idx(self):
        self.event_idx = len(self.event_data) - 1
        self.eventIdx.setPlainText(str(self.event_idx))
        self.switch_event(False)

    def reduce_event_idx(self):
        self.reset_effects()
        self.update_json()
        self.reset_requirements()
        self.statusMessage.setText("")
        if self.event_idx > 0:
            self.event_idx -= 1
            self.eventIdx.setPlainText(str(self.event_idx))
            self.switch_event(False)

    def increase_event_idx(self):
        try:
            self.update_json()
            self.reset_effects()
            self.reset_requirements()
            self.statusMessage.setText("")
            self.event_idx += 1
            self.eventIdx.setPlainText(str(self.event_idx))
            if self.event_idx <= len(self.event_data) - 1:
                self.switch_event(False)
            else:
                self.switch_event(True)
        except Exception as e:
            print(e)

    def init_base_structure(self):
        self.event_data.append(dict())
        self.event_data[self.event_idx]["choices"] = list()

    def update_json(self):
        try:
            self.event_data[self.event_idx]["name"] = self.textName.toPlainText()
            self.event_data[self.event_idx]["description"] = self.textDescription.toPlainText()
            self.event_data[self.event_idx]["isCrisis"] = self.isCrisisCheckbox.isChecked()

            with open(self.selectedFilePath.text(), "w") as f:
                json.dump(self.event_data, f)
            self.statusMessage.setText("SUCCESS:")
        except Exception as e:
            self.statusMessage.setText("ERROR:")
            print(e)



class LeaderEditor(QtWidgets.QMainWindow):
    def __init__(self):
        super(LeaderEditor, self).__init__()
        uic.loadUi('qt_ui/leaderEditor.ui', self)


        self.selectFileButton.clicked.connect(self.select_file)
        self.updateJsonButton.clicked.connect(self.update_json)
        self.reduceButton.clicked.connect(self.reduce_leader_idx)
        self.increaseButton.clicked.connect(self.increase_leader_idx)
        self.addDialogButton.clicked.connect(lambda: self.add_new_conversation(self.typeOfSentence.currentText()))
        self.addEffectButton.clicked.connect(self.add_effect)
        self.resetEffectButton.clicked.connect(self.reset_effects)

        self.RemoveCardButton.clicked.connect(self.remove_card)

        self.addCardButton.clicked.connect(self.add_card)

        self.radioSentenceMod.clicked.connect(self.show_sentence_mod)
        self.radioCardMod.clicked.connect(self.show_card_mod)


        self.addEffectButton.setEnabled(False)
        self.resetEffectButton.setEnabled(False)
        self.updateJsonButton.setEnabled(False)
        self.addCardButton.setEnabled(False)
        self.RemoveCardButton.setEnabled(False)
        self.addDialogButton.setEnabled(False)
        self.reduceButton.setEnabled(False)
        self.increaseButton.setEnabled(False)
        self.show_sentence_mod()
        self.show()

    def remove_card(self):
        if self.textTrigramEdit.toPlainText() != "" and self.textTrigramEdit.toPlainText() in self.leader_data[self.leader_idx]["cards"]:
           del self.leader_data[self.leader_idx]["cards"][self.trigramComboBoxEdit.currentText()]
           self.cards.setPlainText(str(self.leader_data[self.leader_idx]["cards"]))

    def restore_empty_UI(self):
        self.textName.setPlainText("")
        self.textDomainName.setPlainText("")
        self.textMaxSanity.setPlainText("")
        self.textDescription.setPlainText("")

        self.sentencesConversation.setPlainText("")
        self.cards.setPlainText("")
        self.sentencesCrisis.setPlainText("")
        self.sentencesEvent.setPlainText("")
        self.textTrigramEdit.setPlainText("")
        self.textTrigramCard.setPlainText("")
        self.textCardName.setPlainText("")
        self.textCardDescription.setPlainText("")
        self.textEffectValue.setPlainText("")

    def add_card(self):
        try:
            textCardTrigram = self.textTrigramCard.toPlainText()
            self.leader_data[self.leader_idx]["cards"][textCardTrigram] = dict()
            self.leader_data[self.leader_idx]["cards"][textCardTrigram]["name"] = self.textCardName.toPlainText()
            self.leader_data[self.leader_idx]["cards"][textCardTrigram]["description"] = self.textCardDescription.toPlainText()
            self.leader_data[self.leader_idx]["cards"][textCardTrigram]["effects"] = self.effects

            self.cards.setPlainText(str(self.leader_data[self.leader_idx]["cards"]))
        except Exception as e:
            print(e)
    def add_effect(self):
        try:
            if self.textEffectValue.toPlainText() == "":
                self.statusMessage.setText("ERROR: effectKey and effectValue must be filled")
            else:
                self.effects[self.effectKeyComboBox.currentText()] = self.textEffectValue.toPlainText()
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
        self.update_json()
        self.reset_effects()
        self.statusMessage.setText("")
        if self.leader_idx > 0:
            self.leader_idx -= 1
            self.leaderIdx.setPlainText(str(self.leader_idx))
            self.switch_leader()

    def init_base_structure(self):
        self.leader_data.append(dict())
        self.leader_data[self.leader_idx]["sentencesConversation"] = dict()
        self.leader_data[self.leader_idx]["sentencesCrisis"] = dict()
        self.leader_data[self.leader_idx]["sentencesEvent"] = dict()
        self.leader_data[self.leader_idx]["cards"] = dict()

    def increase_leader_idx(self):
        self.update_json()
        self.reset_effects()
        self.statusMessage.setText("")
        self.leader_idx += 1
        self.leaderIdx.setPlainText(str(self.leader_idx))
        if self.leader_idx <= len(self.leader_data) - 1:
            self.switch_leader(False)
        else:
            self.switch_leader(True)

    def select_file(self):
        # fname = QFileDialog.getOpenFileName(self, 'Open file', '', 'Images (*.json)')
        fname = QFileDialog.getOpenFileName(self, 'Open file', '')
        self.selectedFilePath.setText(fname[0])
        if fname[0].endswith('.json'):
            self.load_json(fname[0])
            self.statusMessage.setText("")
        else:
            self.statusMessage.setText("ERROR: Loaded file must .json")
    def switch_leader(self, is_new=False):
        if is_new == False:
            try:
                self.trigram.setCurrentText(self.leader_data[self.leader_idx]["trigram"])
                self.textName.setPlainText(self.leader_data[self.leader_idx]["leaderName"])
                self.textDomainName.setPlainText(self.leader_data[self.leader_idx]["domainName"])
                self.textMaxSanity.setPlainText(str(self.leader_data[self.leader_idx]["maxSanity"]))
                self.textDescription.setPlainText(str(self.leader_data[self.leader_idx]["description"]))

                self.sentencesConversation.setPlainText(str(self.leader_data[self.leader_idx]["sentencesConversation"]))

                self.sentencesCrisis.setPlainText(str(self.leader_data[self.leader_idx]["sentencesCrisis"]))
                self.sentencesEvent.setPlainText(str(self.leader_data[self.leader_idx]["sentencesEvent"]))

                self.cards.setPlainText(str(self.leader_data[self.leader_idx]["cards"]))
                self.reduceButton.setEnabled(True)
                self.increaseButton.setEnabled(True)
            except Exception as e:
                print(e)
        else:
            self.init_base_structure()
            self.restore_empty_UI()

            self.reduceButton.setEnabled(True)
            self.increaseButton.setEnabled(True)
        print(self.leader_data)

    def load_json(self, path):
        self.restore_empty_UI()
        self.leader_idx = 0
        with open(path, "r") as f:
            try:
                self.leader_data = json.load(f)
                self.switch_leader(False)
            except JSONDecodeError:
                self.leader_data = list()
                self.switch_leader(True)

        self.addEffectButton.setEnabled(True)
        self.resetEffectButton.setEnabled(True)
        self.updateJsonButton.setEnabled(True)
        self.addCardButton.setEnabled(True)
        self.RemoveCardButton.setEnabled(True)
        self.addDialogButton.setEnabled(True)

    def update_json(self):
        try:
            self.leader_data[self.leader_idx]["trigram"] = self.trigram.currentText()
            self.leader_data[self.leader_idx]["leaderName"] = self.textName.toPlainText()
            self.leader_data[self.leader_idx]["domainName"] = self.textDomainName.toPlainText()
            self.leader_data[self.leader_idx]["maxSanity"] = self.textMaxSanity.toPlainText()
            self.leader_data[self.leader_idx]["description"] = self.textDescription.toPlainText()



            with open(self.selectedFilePath.text(), "w") as f:
                json.dump(self.leader_data, f)
            self.statusMessage.setText("SUCCESS:")
        except Exception as e:
            self.statusMessage.setText("ERROR:")
            print(e)

    def add_new_conversation(self, typeOfSentence):
        print(typeOfSentence)
        try:
            if self.textDialog.toPlainText() == "" or self.textMaxSanity.toPlainText() == "" or self.textDangerLevel.toPlainText() == "":
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


                self.sentencesConversation.setPlainText(str(self.leader_data[self.leader_idx]["sentencesConversation"]))
                self.sentencesCrisis.setPlainText(str(self.leader_data[self.leader_idx]["sentencesCrisis"]))
                self.sentencesEvent.setPlainText(str(self.leader_data[self.leader_idx]["sentencesEvent"]))
                self.textDialog.setPlainText("")
                self.textDangerLevel.setPlainText("")
                self.statusMessage.setText("SUCCESS:")
        except Exception as e:
            print("error")
            print(e)





if __name__ == '__main__':
    os.environ["QT_AUTO_SCREEN_SCALE_FACTOR"] = "1"
    app = QtWidgets.QApplication(sys.argv)
    window = EditorSelector()
    app.exec_()
