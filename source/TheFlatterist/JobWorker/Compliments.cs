using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobWorker
{
    static class Compliments
    {
        static Random rnd = new Random();

        private static List<string> compliments = new List<string>
        {
            "you have such strong hands.",
            "wow. I mean... just... wow!",
            "your diction is superb.",
            "you have a lot of class.",
            "wow, you cook better than my mom!",
            "you should be running this place.",
            "you are one of my favorite people in the world.",
            "you are the epitome of elegance and grace.",
            "you are awesome!",
            "you always know exactly what to say.",
            "your eyebrows are so well-defined.",
            "I want to be reincarnated as you.",
            "when you smile, you light up the entire room.",
            "have you ever considered sharing your talents in a variety show?",
            "your hair looks really nice like that.",
            "I'd love to be your partner in Trivial Pursuit.",
            "those jeans make your butt look great.",
            "in case I haven't mentioned it lately: you rock!",
            "your posture is perfect. Do you work out?",
            "you smell wonderful, considering what you've been through.",
            "you are so hip.",
            "you have impeccable table manners.",
            "you really know your stuff.",
            "why can't everyone be more like you?",
            "you are so much fun to be with!",
            "I love the sound of your laugh.",
            "you're one of a kind.",
            "I think you're brilliant.",
            "your teeth are so white.",
            "you say the funniest things!",
            "you have a wide variety of useful skills.",
            "you are the Vincent Van Gogh of driving.",
            "you have beautiful toenails.",
            "I like the way your mind works.",
            "you are a rock star!",
            "your eyes are like diamonds.",
            "you smell like a million bucks.",
            "your expertise in Civil War history is mindblowing.",
            "have you lost weight?",
            "your personal hygiene is above reproach.",
            "you have lobster-like tenacity.",
            "you're the Dalai Lama of breakdancing.",
            "wow. You sure do know how to whistle. You remind me of Axl Rose.",
            "you are a person of unparalleled levels of cobernosity.",
            "I never would have thought of that. You're so clever!",
            "you are so sweet you could put sugar out of business.",
            "there's no one quite like you.",
            "you are such a good driver that you deserve to cut me off.",
            "your hair withstands humidity so well.",
            "I follow you on Twitter.",
            "your salmon-poaching skills are top notch.",
            "a barrel of monkeys has nothing on you.",
            "your bone structure is marvelous.",
            "can I ask you to never leave?",
            "well aren't you just a breath of hot air?",
            "you are a fabulous creature of divine beauty.",
            "you are a majestic unicorn."
        };

        public static string GetRandomCompliment()
        {
            int r = rnd.Next(compliments.Count);
            return compliments[r];
        }
    }
}
