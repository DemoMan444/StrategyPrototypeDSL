package org.example.games.generator

import org.eclipse.emf.ecore.resource.Resource
import org.eclipse.xtext.generator.AbstractGenerator
import org.eclipse.xtext.generator.IFileSystemAccess2
import org.eclipse.xtext.generator.IGeneratorContext
import org.example.games.games.Model
import org.example.games.games.Game
import org.example.games.games.Setup
import org.example.games.games.Player
import org.example.games.games.IntVar
import org.example.games.games.VarKind
import org.example.games.games.Rule
import org.example.games.games.Condition
import org.example.games.games.Decisions
import org.example.games.games.Decision
import org.example.games.games.Script
import org.example.games.games.Turns
import org.example.games.games.Expression
import org.example.games.games.LValue
import org.example.games.games.PlayerVarLValue
import org.example.games.games.VarLValue
import org.example.games.games.IntConstant
import org.example.games.games.BoolConstant
import org.example.games.games.Not
import org.example.games.games.OrExpression
import org.example.games.games.AndExpression
import org.example.games.games.ComparisonExpression
import org.example.games.games.Addition
import org.example.games.games.Multiplication
import org.example.games.games.Primary

class GamesGenerator extends AbstractGenerator {

    override void doGenerate(Resource resource, IFileSystemAccess2 fsa, IGeneratorContext context) {
        val model = resource.contents.head as Model
        
        for (top : model.elements) {
            switch top {
                Game:       generateGame(top, fsa)
                Setup:      generateSetup(top, fsa)
                Player:     generatePlayer(top, fsa)
                Rule:       generateRule(top, fsa)
                Condition:  generateCondition(top, fsa)
                Decisions:  generateDecisions(top, fsa)
                Decision:   generateDecision(top, fsa)
                Script:     generateScript(top, fsa)
            }
        }
        
        // Generate common utilities / interpreter
        generateGameEngine(fsa)
        generateExpressionEvaluator(fsa)
    }

    def generateGame(Resource game, IFileSystemAccess2 fsa) '''
        package org.example.games.generated;

        public class «game.name.toFirstUpper»Game {
            private Setup setup;
            private Rule rule;
            private Decisions decisions;
            private Script script;
            
            public «game.name.toFirstUpper»Game() {
                this.setup = new «game.setup.name.toFirstUpper»Setup();
                this.rule = new «game.rule.name.toFirstUpper»Rule();
                this.decisions = new «game.decisions.name.toFirstUpper»Decisions();
                this.script = new «game.script.name.toFirstUpper»Script();
            }
            
            public void run() {
                System.out.println("Starting game: «game.name»");
                script.execute(setup);
                
                // Simple win/lose check after script (can be improved)
                if (rule.checkWinCondition(setup)) {
                    System.out.println("Player 1 wins!");
                } else if (rule.checkLoseCondition(setup)) {
                    System.out.println("Player 1 loses!");
                } else {
                    System.out.println("Game ended without clear winner.");
                }
            }
        }
    '''

    def generateSetup(Setup s, IFileSystemAccess2 fsa) '''
        package org.example.games.generated;

        public class «s.name.toFirstUpper»Setup {
            «FOR v : s.vars»
                «v.generateField»
            «ENDFOR»
            
            «IF s.limitName !== null»
            private int «s.limitName»Limit;
            «ENDIF»
            
            private Player player1 = new Player1();
            private Player player2 = new Player2();
            
            public Player getPlayer1() { return player1; }
            public Player getPlayer2() { return player2; }
            
            «s.generateInitMethod»
        }
    '''

    def generatePlayer(Player p, IFileSystemAccess2 fsa) '''
        package org.example.games.generated;

        public class «p.name.toFirstUpper» {
            private int health = «p.health»;
            
            «FOR v : p.vars»
                «v.generateField»
            «ENDFOR»
            
            «FOR r : p.resources»
                «r.generateField»
            «ENDFOR»
            
            «IF p.militaryResources !== null»
                «FOR m : p.militaryResources»
                    «m.generateField»
                «ENDFOR»
            «ENDIF»
            
            public int getHealth() { return health; }
            public void setHealth(int h) { this.health = h; }
            
            «FOR v : p.vars + p.resources + (p.militaryResources ?: #[])»
                «v.generateGetterSetter»
            «ENDFOR»
        }
    '''

    def generateField(IntVar v) '''
        «switch v.kind {
            case VAR: '''private int «v.name»;'''
            case RESOURCE: '''private int «v.name»;'''
            case MILITARY: '''private int «v.name»;'''
        }»
    '''

    def generateGetterSetter(IntVar v) '''
        public int get«v.name.toFirstUpper»() { return «v.name»; }
        public void set«v.name.toFirstUpper»(int value) { this.«v.name» = value; }
    '''

    def generateInitMethod(Setup s) '''
        public void initialize() {
            «FOR v : s.vars»
                «IF v.init !== null»
                this.«v.name» = «v.init.generateExpression»;
                «ENDIF»
            «ENDFOR»
            «IF s.limitName !== null»
            // timelimit «s.limitName» not yet initialized
            «ENDIF»
        }
    '''

    def generateRule(Rule r, IFileSystemAccess2 fsa) '''
        package org.example.games.generated;

        public class «r.name.toFirstUpper»Rule {
            public boolean checkWinCondition(Setup setup) {
                «FOR c : r.winConditions»
                    if («c.name.toFirstLower».evaluate(setup)) return true;
                «ENDFOR»
                return false;
            }
            
            public boolean checkLoseCondition(Setup setup) {
                «FOR c : r.loseConditions»
                    if («c.name.toFirstLower».evaluate(setup)) return true;
                «ENDFOR»
                return false;
            }
        }
    '''

    def generateCondition(Condition c, IFileSystemAccess2 fsa) '''
        package org.example.games.generated;

        public class «c.name.toFirstUpper»Condition {
            public boolean evaluate(Setup setup) {
                return «c.boolexp.generateExpression»;
            }
        }
    '''

    def generateDecisions(Decisions d, IFileSystemAccess2 fsa) '''
        package org.example.games.generated;

        public class «d.name.toFirstUpper»Decisions {
            «FOR dec : d.decisions»
            public static final String «dec.name.toUpperCase» = "«dec.name»";
            «ENDFOR»
        }
    '''

    def generateDecision(Decision d, IFileSystemAccess2 fsa) '''
        package org.example.games.generated;

        public class «d.name.toFirstUpper»Decision {
            public void execute(Setup setup) {
                «d.body.generateBlock»
            }
        }
    '''

    def generateScript(Script s, IFileSystemAccess2 fsa) '''
        package org.example.games.generated;

        public class «s.name.toFirstUpper»Script {
            public void execute(Setup setup) {
                «FOR t : s.turns»
                    System.out.println("Turn «t.index»: " + "«t.player.name» plays decision «t.decision.name»");
                    new «t.decision.name.toFirstUpper»Decision().execute(setup);
                «ENDFOR»
            }
        }
    '''

    def generateBlock(Block b) '''
        «FOR stmt : b.statements»
            «switch stmt {
                ExpressionStmt: stmt.expr.generateExpression + ";"
                Assignment:     stmt.target.generateLValue + " = " + stmt.expression.generateExpression + ";"
                PrintStmt:      '''System.out.println(«stmt.value»);'''
                IntVar:         "// local var " + stmt.name + " not yet supported"
                Conditional:    stmt.generateConditional
                default:        "// unsupported statement"
            }»
        «ENDFOR»
    '''

    def generateConditional(Conditional c) '''
        if («c.condition.generateExpression») «c.thenBlock.generateBlock»
        «FOR i : 0 ..< c.elifConditions.size»
            else if («c.elifConditions.get(i).generateExpression») «c.elifBlocks.get(i).generateBlock»
        «ENDFOR»
        «IF c.elseBlock !== null»
            else «c.elseBlock.generateBlock»
        «ENDIF»
    '''

    def dispatch CharSequence generateExpression(Expression e) '''/* unimplemented */'''

    def dispatch CharSequence generateExpression(OrExpression e) '''
        «e.left.generateExpression» || «e.right?.generateExpression»
    '''

    def dispatch CharSequence generateExpression(AndExpression e) '''
        «e.left.generateExpression» && «e.right?.generateExpression»
    '''

    def dispatch CharSequence generateExpression(ComparisonExpression e) '''
        «e.left.generateExpression» «e.op» «e.right?.generateExpression»
    '''

    def dispatch CharSequence generateExpression(Addition e) '''
        «e.left.generateExpression» «IF e.^operator == '+'»+«ELSE»-«ENDIF» «e.right?.generateExpression»
    '''

    def dispatch CharSequence generateExpression(Multiplication e) '''
        «e.left.generateExpression» «IF e.^operator == '*'»*«ELSE»/«ENDIF» «e.right?.generateExpression»
    '''

    def dispatch CharSequence generateExpression(Primary p) {
        switch p {
            IntConstant:     p.value.toString
            BoolConstant:    p.value
            LValueExpr:      p.target.generateLValue
            Not:             "!" + p.expression.generateExpression
            Primary:         "(" + p.expression.generateExpression + ")"
            default:         "/* unknown primary */"
        }
    }

    def generateLValue(LValue lv) {
        switch lv {
            PlayerVarLValue: '''«lv.player.name.toFirstLower».get«lv.varName.toFirstUpper»()'''
            VarLValue:       lv.varName
        }
    }

    def generateGameEngine(IFileSystemAccess2 fsa) '''
        package org.example.games.generated;

        public class GameEngine {
            public static void main(String[] args) {
                «/* Assume first game — in real project you'd choose or register games */»
                «IF fsa.isFile("src-gen/org/example/games/generated/FirstGameGame.java")»
                new FirstGameGame().run();
                «ELSE»
                System.out.println("No game found.");
                «ENDIF»
            }
        }
}